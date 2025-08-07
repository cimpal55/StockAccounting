using Microsoft.AspNetCore.Mvc;
using Serilog;
using StockAccounting.Web.Constants;
using StockAccounting.Web.Extensions;
using StockAccounting.Web.Services.Interfaces;
using static LinqToDB.Common.Configuration;
using System.Drawing;
using StockAccounting.Core.Android.Data.Data.Repositories;
using StockAccounting.Core.Data.Enums;
using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.EmployeeData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.Data.StockEmployees;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Core.Data.Services.Interfaces;
using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Web.Controllers
{
    public class DocumentDataController : BaseController
    {
        private readonly ILogger<DocumentDataController> _logger;
        private readonly IPaginationService _paginationService;
        private readonly IDocumentDataRepository _documentDataRepository;
        private readonly IGenericRepository<DocumentDataBaseModel> _repository;
        private readonly IGenericRepository<StockEmployeesBaseModel> _gStockEmployeesRepository;
        private readonly IGenericRepository<ScannedDataBaseModel> _gScannedDataRepository;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly IEmployeeDataRepository _employeeRepository;
        private readonly IExternalDataRepository _externalDataRepository;
        private readonly IStockDataRepository _stockDataRepository;
        public DocumentDataController(
            ILogger<DocumentDataController> logger,
            IPaginationService paginationService,
            IDocumentDataRepository repository,
            IEmployeeDataRepository employeeRepository,
            IExternalDataRepository externalDataRepository,
            IStockDataRepository stockDataRepository,
            IGenericRepository<DocumentDataBaseModel> grepository,
            IGenericRepository<StockEmployeesBaseModel> gStockEmployeesRepository,
            IGenericRepository<ScannedDataBaseModel> gScannedDataRepository,
            ISmtpEmailService smtpEmailService)
        {
            _logger = logger;
            _paginationService = paginationService;
            _documentDataRepository = repository;
            _employeeRepository = employeeRepository;
            _externalDataRepository = externalDataRepository;
            _stockDataRepository = stockDataRepository;
            _repository = grepository;
            _gStockEmployeesRepository = gStockEmployeesRepository;
            _gScannedDataRepository = gScannedDataRepository;
            _smtpEmailService = smtpEmailService;
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> List([FromForm] DocumentDataTableRequest request)
        {
            if (request.PageId == 0) request.PageId = 1;
            if (request.PageId < 0) return BadRequest();

            var sortColumn = string.IsNullOrEmpty(request.SortColumn) ? "Created" : request.SortColumn;
            var sortDirection = string.IsNullOrEmpty(request.SortDirection) ? "desc" : request.SortDirection;

            var externalData = await _externalDataRepository.GetExternalDataAsync();
            var employees = await _employeeRepository.GetEmployeesAsync();

            var data = await _paginationService.PaginatedDocumentsSorted(
                request.PageId,
                request.Length > 0 ? request.Length : _itemsPerPage,
                request.SearchText,
                sortColumn,
                sortDirection
            );

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    draw = request.Draw,
                    recordsTotal = data.TotalData,
                    recordsFiltered = data.TotalData,
                    data = data.Select(d => new
                    {
                        Employee1 = d.Employee1,
                        Employee2 = d.Employee2,
                        Employee1Id = d.Employee1Id,
                        Employee2Id = d.Employee2Id,
                        Created = d.Created,
                        id = d.Id
                    })
                });
            }

            var viewModel = new DocumentDataViewModel
            {
                DocumentDataModel = data,
                Employees = employees,
                ExternalData = externalData,
                TotalPages = data.TotalPages,
                PageIndex = data.PageIndex,
                TotalData = data.TotalData,
                StartPage = request.PageId >= _pagesInRow ? request.PageId - 2 : 1,
                EndPage = request.PageId >= _pagesInRow && request.PageId < data.TotalPages
                    ? request.PageId + 1
                    : data.TotalPages > _pagesInRow && request.PageId < data.TotalPages
                        ? _pagesInRow
                        : data.TotalPages,
                SearchText = request.SearchText,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };

            return View(viewModel);
        }


        [HttpPost("InsertDocumentData")]
        public async Task<ActionResult> InsertDocumentData([Bind("Employee1Id, Employee2Id")] DocumentDataViewModel item)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                       .Where(y => y.Count > 0)
                       .ToList();
                this.AddAlertDanger($"{WebConstants.Error}. {errors}.");
                return BadRequest();
            }

            var pages = await _paginationService.PaginatedDocuments(1, _itemsPerPage);
            var pageId = pages.TotalData % _itemsPerPage == 0 ? pages.TotalPages + 1 : pages.TotalPages;
            var isSynchronization = await _documentDataRepository.CheckDocumentSynchronizationAsync(item.Employee2Id);

            var DocumentDataBaseModel = new DocumentDataBaseModel
            {
                Id = item.Id,
                Employee1Id = item.Employee1Id,
                Employee2Id = item.Employee2Id,
                IsSynchronization = isSynchronization,
                DocumentType = isSynchronization == true ? (int)StockTypes.Taken : (int)StockTypes.Returned,
                ManuallyAdded = true,
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            try
            {
                Log.Information("WEB_InsertDocumentData {@Data}", DocumentDataBaseModel);
                Log.CloseAndFlush();
                await _repository.InsertAsync(DocumentDataBaseModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            this.AddAlertSuccess(WebConstants.Success);
            return Redirect($"DocumentData");
        }

        [HttpPost("UpdateDocumentData")]
        public async Task<ActionResult> UpdateDocumentData(
            [Bind("Id, Employee1Id, Employee2Id, Created, PageId")] DocumentDataViewModel item)
        {
            if (!ModelState.IsValid)
            {
                this.AddAlertDanger("Error: Document document is not updated!");
                return BadRequest();
            }

            try
            {
                var DocumentDataBaseModel = new DocumentDataBaseModel
                {
                    Id = item.Id,
                    Employee1Id = item.Employee1Id,
                    Employee2Id = item.Employee2Id,
                    Updated = DateTime.Now,
                    Created = item.Created,
                };

                if (_documentDataRepository.CheckIfDocumentHasScannedData(item.Id))
                {
                    var scannedData = await _documentDataRepository.ReturnDocumentScannedData(item.Id);
                    List<StockEmployeesBaseModel> stockData = new();
                    stockData = await _documentDataRepository.ReturnStockEmployeesByDocumentNumber(scannedData[0]);

                    for (int i = 0; i < stockData.Count; i++)
                    {
                        stockData[i].Created = item.Created;
                        await _gStockEmployeesRepository.UpdateAsync(stockData[i]);
                    }

                    await _repository.UpdateAsync(DocumentDataBaseModel);
                }
                else
                {
                    Log.Information("WEB_UpdateDocumentData {@Data}", DocumentDataBaseModel);
                    Log.CloseAndFlush();
                    await _documentDataRepository.UpdateDocumentDataAsync(DocumentDataBaseModel);
                }
            }
            catch (Exception e)
            {
                this.AddAlertDanger($"{WebConstants.Error} {e.Message}.");
                _logger.LogError(e.Message);
                return BadRequest();
            }

            this.AddAlertSuccess(WebConstants.Success);
            return Redirect($"DocumentData?pageId={item.PageId}");
        }

        [HttpPost("DeleteDocumentData")]
        public async Task<ActionResult> DeleteDocumentData(DocumentDataBaseModel item)
        {
            try
            {
                if (_documentDataRepository.CheckIfDocumentHasScannedData(item.Id))
                {
                    var scannedData = await _documentDataRepository.ReturnDocumentScannedData(item.Id);
                    var document = await _documentDataRepository.GetDocumentDataByIdAsync(item.Id);
                    List<StockEmployeesBaseModel> stockData = new();

                    if (document.DocumentType != (int)StockTypes.Inventory)
                        stockData = await _documentDataRepository.ReturnStockEmployeesByDocumentNumber(scannedData[0]);
                    else
                        stockData = await _documentDataRepository.ReturnStockEmployeesBySerialNumber(scannedData[0]
                            .DocumentSerialNumber);

                    foreach (var data in scannedData)
                    {
                        await _gScannedDataRepository.DeleteAsync(data);
                    }

                    foreach (var t in stockData)
                    {
                        await _stockDataRepository.UpdateStockQuantity(t.StockDataId, t.Quantity * -1);
                        await _gStockEmployeesRepository.DeleteAsync(t);
                    }

                    await _repository.DeleteAsync(item);
                    this.AddAlertSuccess(WebConstants.Success);
                    return Ok();
                }
                else
                {
                    await _repository.DeleteAsync(item);
                    this.AddAlertSuccess(WebConstants.Success);
                    return Ok();
                }
            }
            catch (Exception e)
            {
                this.AddAlertDanger($"{WebConstants.Error} {e.Message}.");
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }

    }
}
