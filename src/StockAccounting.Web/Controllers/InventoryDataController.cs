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

namespace StockAccounting.Web.Controllers
{
    public class InventoryDataController : BaseController
    {
        private readonly ILogger<InventoryDataController> _logger;
        private readonly IPaginationService _paginationService;
        private readonly IInventoryDataRepository _inventoryDataRepository;
        private readonly IGenericRepository<InventoryDataBaseModel> _repository;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly IEmployeeDataRepository _employeeRepository;
        private readonly IExternalDataRepository _externalDataRepository;
        public InventoryDataController(
            ILogger<InventoryDataController> logger,
            IPaginationService paginationService,
            IInventoryDataRepository repository,
            IEmployeeDataRepository employeeRepository,
            IExternalDataRepository externalDataRepository,
            IGenericRepository<InventoryDataBaseModel> grepository,
            ISmtpEmailService smtpEmailService)
        {
            _logger = logger;
            _paginationService = paginationService;
            _inventoryDataRepository = repository;
            _employeeRepository = employeeRepository;
            _externalDataRepository = externalDataRepository;
            _repository = grepository;
            _smtpEmailService = smtpEmailService;
        }
        public async Task<IActionResult> List(int pageId, string searchText)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            PaginatedData<InventoryDataModel> items;

            if (string.IsNullOrEmpty(searchText) || searchText == "dummyText")
                items = await _paginationService.PaginatedDocuments(pageId, _itemsPerPage);
            else
                items = await _paginationService.PaginatedSearchedDocuments(pageId, _itemsPerPage, searchText);

            var employees = await _employeeRepository.GetEmployeesAsync();
            var externalData = await _externalDataRepository.GetExternalDataAsync();

            var data = new InventoryDataViewModel()
            {
                InventoryDataModel = items,
                Employees = employees,
                ExternalData = externalData,
                TotalPages = items.TotalPages,
                PageIndex = items.PageIndex,
                TotalData = items.TotalData,
                StartPage = pageId >= _pagesInRow ? pageId - 2 : 1,
                EndPage = pageId >= _pagesInRow && pageId < items.TotalPages ? pageId + 1 : items.TotalPages > _pagesInRow && pageId < items.TotalPages ? _pagesInRow : items.TotalPages,
                SearchText = searchText
            };

            return View(data);
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
                this.AddAlertSuccess(WebConstants.Success);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return Redirect($"InventoryData?pageId={pageId}");
        }

        [HttpPost("UpdateInventoryData")]
        public async Task<ActionResult> UpdateInventoryData(
            [Bind("Id, Employee1Id, Employee2Id, PageId")] InventoryDataViewModel item)
        {
            if (!ModelState.IsValid)
            {
                this.AddAlertDanger("Error: inventory document is not updated!");
                return BadRequest();
            }

            var inventoryDataBaseModel = new InventoryDataBaseModel
            {
                Id = item.Id,
                Employee1Id = item.Employee1Id,
                Employee2Id = item.Employee2Id,
            };

            try
            {
                Log.Information("WEB_UpdateInventoryData {@Data}", inventoryDataBaseModel);
                Log.CloseAndFlush();
                await _inventoryDataRepository.UpdateInventoryDataAsync(inventoryDataBaseModel);
                this.AddAlertSuccess(WebConstants.Success);
            }
            catch (Exception e)
            {
                this.AddAlertDanger($"{WebConstants.Error} {e.Message}.");
                _logger.LogError(e.Message);
            }

            return Redirect($"InventoryData?pageId={item.PageId}");
        }

        [HttpPost("DeleteInventoryData")]
        public async Task<ActionResult> DeleteInventoryData(InventoryDataBaseModel item)
        {
            if (!_inventoryDataRepository.CheckIfDocumentHasScannedData(item.Id))
            {
                try
                {
                    await _repository.DeleteAsync(item);
                    this.AddAlertSuccess(WebConstants.Success);

                    return Ok();
                }
                catch (Exception e)
                {
                    this.AddAlertDanger($"{WebConstants.Error} {e.Message}.");
                    _logger.LogError(e.Message);
                }
            }

            this.AddAlertWarning("Warning: inventory document has attached scanned data!");

            return BadRequest();
        }

    }
}
