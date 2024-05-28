using Microsoft.AspNetCore.Mvc;
using Serilog;
using StockAccounting.Web.Constants;
using StockAccounting.Web.Extensions;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Web.Services.Interfaces;
using StockAccounting.Web.Utils;
using StockAccounting.Web.ViewModels;
using StockAccounting.Core.Data.Repositories.Interfaces;
using static LinqToDB.Common.Configuration;
using System.Drawing;
using StockAccounting.Core.Data.Services.Interfaces;
using StockAccounting.Core.Data.Repositories;

namespace StockAccounting.Web.Controllers
{
    public class InventoryDataController : BaseController
    {
        private readonly ILogger<InventoryDataController> _logger;
        private readonly IPaginationService _paginationService;
        private readonly IInventoryDataRepository _inventoryDataRepository;
        private readonly IGenericRepository<InventoryDataBaseModel> _repository;
        private readonly IGenericRepository<StockEmployeesBaseModel> _gStockEmployeesRepository;
        private readonly IGenericRepository<ScannedDataBaseModel> _gScannedDataRepository;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly IEmployeeDataRepository _employeeRepository;
        private readonly IExternalDataRepository _externalDataRepository;
        private readonly IStockDataRepository _stockDataRepository;
        public InventoryDataController(
            ILogger<InventoryDataController> logger,
            IPaginationService paginationService,
            IInventoryDataRepository repository,
            IEmployeeDataRepository employeeRepository,
            IExternalDataRepository externalDataRepository,
            IStockDataRepository stockDataRepository,
            IGenericRepository<InventoryDataBaseModel> grepository,
            IGenericRepository<StockEmployeesBaseModel> gStockEmployeesRepository,
            IGenericRepository<ScannedDataBaseModel> gScannedDataRepository,
            ISmtpEmailService smtpEmailService)
        {
            _logger = logger;
            _paginationService = paginationService;
            _inventoryDataRepository = repository;
            _employeeRepository = employeeRepository;
            _externalDataRepository = externalDataRepository;
            _stockDataRepository = stockDataRepository;
            _repository = grepository;
            _gStockEmployeesRepository = gStockEmployeesRepository;
            _gScannedDataRepository = gScannedDataRepository;
            _smtpEmailService = smtpEmailService;
        }

        public async Task<IActionResult> List(int pageId, string searchText, string sortBy)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            PaginatedData<InventoryDataModel> data;
            IEnumerable<ExternalDataModel> externalData = await _externalDataRepository.GetExternalDataAsync();
            IEnumerable<EmployeeDataModel> employees = await _employeeRepository.GetEmployeesAsync();

            if (string.IsNullOrEmpty(searchText) || searchText == "dummyText")
                data = await _paginationService.PaginatedDocuments(pageId, _itemsPerPage);
            else
                data = await _paginationService.PaginatedSearchedDocuments(pageId, _itemsPerPage, searchText);

            var inventoryDataViewModel = new InventoryDataViewModel
            {
                InventoryDataModel = data,
                ExternalData = externalData,
                Employees = employees,
                TotalPages = data.TotalPages,
                PageIndex = data.PageIndex,
                TotalData = data.TotalData,
                StartPage = pageId >= _pagesInRow ? pageId - 2 : 1,
                EndPage = pageId >= _pagesInRow && pageId < data.TotalPages ? pageId + 1 : data.TotalPages > _pagesInRow && pageId < data.TotalPages ? _pagesInRow : data.TotalPages,
                SearchText = searchText
            };

            return View(inventoryDataViewModel);
        }

        [HttpPost("InsertInventoryData")]
        public async Task<ActionResult> InsertInventoryData([Bind("Employee1Id, Employee2Id")] InventoryDataViewModel item)
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

            var inventoryDataBaseModel = new InventoryDataBaseModel
            {
                Id = item.Id,
                Employee1Id = item.Employee1Id,
                Employee2Id = item.Employee2Id,
                IsSynchronization = await _inventoryDataRepository.CheckInventorySynchronizationAsync(item.Employee2Id),
                ManuallyAdded = true,
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            try
            {
                Log.Information("WEB_InsertInventoryData {@Data}", inventoryDataBaseModel);
                Log.CloseAndFlush();
                await _repository.InsertAsync(inventoryDataBaseModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            this.AddAlertSuccess(WebConstants.Success);
            return Redirect($"InventoryData?pageId={pageId}");
        }

        [HttpPost("UpdateInventoryData")]
        public async Task<ActionResult> UpdateInventoryData(
            [Bind("Id, Employee1Id, Employee2Id, Created, PageId")] InventoryDataViewModel item)
        {
            if (!ModelState.IsValid)
            {
                this.AddAlertDanger("Error: inventory document is not updated!");
                return BadRequest();
            }

            try
            {
                var inventoryDataBaseModel = new InventoryDataBaseModel
                {
                    Id = item.Id,
                    Employee1Id = item.Employee1Id,
                    Employee2Id = item.Employee2Id,
                    Updated = DateTime.Now,
                    Created = item.Created,
                };

                if (_inventoryDataRepository.CheckIfDocumentHasScannedData(item.Id))
                {
                    var scannedData = await _inventoryDataRepository.ReturnDocumentScannedData(item.Id);
                    List<StockEmployeesBaseModel> stockData = new();
                    stockData = await _inventoryDataRepository.ReturnStockEmployeesByDocumentNumber(scannedData[0]);

                    for (int i = 0; i < stockData.Count; i++)
                    {
                        stockData[i].Created = item.Created;
                        await _gStockEmployeesRepository.UpdateAsync(stockData[i]);
                    }

                    await _repository.UpdateAsync(inventoryDataBaseModel);
                }
                else
                {
                    Log.Information("WEB_UpdateInventoryData {@Data}", inventoryDataBaseModel);
                    Log.CloseAndFlush();
                    await _inventoryDataRepository.UpdateInventoryDataAsync(inventoryDataBaseModel);
                }
            }
            catch (Exception e)
            {
                this.AddAlertDanger($"{WebConstants.Error} {e.Message}.");
                _logger.LogError(e.Message);
                return BadRequest();
            }

            this.AddAlertSuccess(WebConstants.Success);
            return Redirect($"InventoryData?pageId={item.PageId}");
        }

        [HttpPost("DeleteInventoryData")]
        public async Task<ActionResult> DeleteInventoryData(InventoryDataBaseModel item)
        {
            try
            {
                if (_inventoryDataRepository.CheckIfDocumentHasScannedData(item.Id))
                {
                    var scannedData = await _inventoryDataRepository.ReturnDocumentScannedData(item.Id);
                    List<StockEmployeesBaseModel> stockData = new();
                    int sign = await _inventoryDataRepository.ReturnInventorySynchronizationAsync(item.Id) ? -1 : 1;
                    stockData = await _inventoryDataRepository.ReturnStockEmployeesByDocumentNumber(scannedData[0]);

                    foreach (var data in scannedData) 
                    {
                        await _gScannedDataRepository.DeleteAsync(data);
                    }

                    for (int i = 0; i < stockData.Count; i++)
                    {
                        await _stockDataRepository.UpdateStockQuantity(stockData[i].StockDataId, stockData[i].Quantity * sign);
                        await _gStockEmployeesRepository.DeleteAsync(stockData[i]);
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
