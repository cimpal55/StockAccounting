﻿using Microsoft.AspNetCore.Mvc;
using Serilog;
using StockAccounting.Core.Data;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Core.Data.Services.Interfaces;
using StockAccounting.Web.Constants;
using StockAccounting.Web.Extensions;
using StockAccounting.Web.Services.Interfaces;
using StockAccounting.Web.ViewModels;

namespace StockAccounting.Web.Controllers
{
    public class ScannedDataController : BaseController
    {
        private readonly IPaginationService _paginationService;
        private readonly ILogger<ScannedDataController> _logger;
        private readonly IGenericRepository<ScannedDataBaseModel> _repository;
        private readonly IExternalDataRepository _externalDataRepository;
        private readonly IScannedDataRepository _scannedDataRepository;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly IStockDataRepository _stockDataRepository;
        public ScannedDataController(
            IPaginationService paginationService,
            ILogger<ScannedDataController> logger,
            IGenericRepository<ScannedDataBaseModel> repository,
            IExternalDataRepository externalDataRepository,
            IScannedDataRepository scannedDataRepository,
            ISmtpEmailService smtpEmailService,
            IStockDataRepository stockDataRepository)
        {
            _paginationService = paginationService;
            _logger = logger;
            _repository = repository;
            _externalDataRepository = externalDataRepository;
            _scannedDataRepository = scannedDataRepository;
            _smtpEmailService = smtpEmailService;
            _stockDataRepository = stockDataRepository;
        }
        public async Task<IActionResult> Details(int pageId, int id)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            var items = await _paginationService.PaginatedScannedData(pageId, _itemsPerPage, id);
            var externalData = await _externalDataRepository.GetExternalDataAsync();

            var data = new ScannedDataViewModel
            {
                ScannedDataModel = items,
                InventoryDataId = id,
                ExternalData = externalData,
                TotalPages = items.TotalPages,
                PageIndex = items.PageIndex,
                TotalData = items.TotalData,
                StartPage = pageId >= _pagesInRow ? pageId - 2 : 1,
                EndPage = pageId >= _pagesInRow && pageId < items.TotalPages ? pageId + 1 : items.TotalPages > _pagesInRow && pageId < items.TotalPages ? _pagesInRow : items.TotalPages,
            };

            return View(data);
        }
        public async Task<JsonResult> GetExternalData(string searchString)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var externalData = await _externalDataRepository.GetExternalDataAsync();

                var list = externalData.Select(x => new { text = string.Join(" ", x.Name, x.ItemNumber, x.PluCode), id = x.Id });

                return Json(list);
            }

            return Json(null);
        }
        [HttpPost("InsertScannedData")]
        public async Task<ActionResult> InsertScannedData([Bind("ExternalDataId, InventoryDataId, Quantity")] ScannedDataViewModel item)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                       .Where(y => y.Count > 0)
                       .ToList();
                this.AddAlertDanger($"{WebConstants.Error}. {errors}.");
                return BadRequest();
            }

            bool isSynchronization;
            var pages = await _paginationService.PaginatedDocuments(1, _itemsPerPage);
            var pageId = pages.TotalData % _itemsPerPage == 0 ? pages.TotalPages + 1 : pages.TotalPages;

            var scannedDataBaseModel = new ScannedDataBaseModel
            {
                Id = item.Id,
                ExternalDataId = item.ExternalDataId,
                InventoryDataId = item.InventoryDataId,
                Quantity = item.Quantity,
                Created = DateTime.Now  
            };

            try 
            {
                Log.Information("WEB_InsertScannedData {@Data}", scannedDataBaseModel);
                Log.CloseAndFlush();
                await _repository.InsertAsync(scannedDataBaseModel);

                isSynchronization = _scannedDataRepository.IsSynchronizationDocument(item.InventoryDataId);

                List<ScannedDataBaseModel> scannedData = new()
                {
                    scannedDataBaseModel
                };

                var stockEmployeeData = new StockEmployeesModel()
                {
                    IsSynchronization = isSynchronization,
                    ExternalDataId = item.ExternalDataId,
                    EmployeeId = _scannedDataRepository.ReturnDocumentEmployees(item.InventoryDataId),
                    StockTypeId = isSynchronization ? (int)StockTypes.Accepted : (int)StockTypes.Returned,
                    Quantity = item.Quantity,
                    Created = DateTime.Now,
                };

                await _stockDataRepository.InsertStockData(stockEmployeeData);

                var emailTo = _scannedDataRepository.ReturnEmployeeNotificationEmail(stockEmployeeData.EmployeeId);

                if(isSynchronization)
                    _smtpEmailService.SendEmail(emailTo, scannedData);

                this.AddAlertSuccess(WebConstants.Success);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return Redirect($"ScannedData/Details/{item.InventoryDataId}");
        }

        [HttpPost("UpdateScannedData")]
        public async Task<ActionResult> UpdateScannedData(
            [Bind("Id, InventoryDataId, ExternalDataId, Quantity, PageId")] ScannedDataViewModel item)
        {
            if (!ModelState.IsValid)
            {
                this.AddAlertDanger(WebConstants.Error);
                return BadRequest();
            }

            var scannedDataBaseModel = new ScannedDataBaseModel
            {
                Id = item.Id,
                ExternalDataId = item.ExternalDataId,
                Quantity = item.Quantity
            };

            try
            {
                Log.Information("WEB_UpdateScannedData {@Data}", scannedDataBaseModel);
                Log.CloseAndFlush();
                await _scannedDataRepository.UpdateScannedDataAsync(scannedDataBaseModel);

                //var stockDataBaseModel = new StockDataBaseModel
                //{
                //    IsSynchronization = _scannedDataRepository.IsSynchronizationDocument(item.InventoryDataId),
                //    EmployeeId = _scannedDataRepository.ReturnDocumentEmployees(item.InventoryDataId),
                //    ExternalDataId = item.ExternalDataId,
                //    Quantity = item.Quantity,
                //};

                //await _stockDataRepository.InsertStockData(stockDataBaseModel);
                this.AddAlertSuccess(WebConstants.Success);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                this.AddAlertDanger($"{WebConstants.Error} {e.Message}.");
            }

            return Redirect($"ScannedData/Details/{item.InventoryDataId}?pageId={item.PageId}");
        }

        [HttpPost("DeleteScannedData")]
        public async Task<ActionResult> DeleteScannedData(ScannedDataBaseModel item)
        {
            try
            {
                await _repository.DeleteAsync(item);
                this.AddAlertSuccess("Scanned data is successfully deleted!");

                return Ok();
            }
            catch (Exception e)
            {
                this.AddAlertDanger($"{WebConstants.Error} {e.Message}.");
                _logger.LogError(e.Message);
            }

            return BadRequest();
        }

        public async Task<JsonResult> GetAutoCompleteExternalAsync()
        {
            var results = await _externalDataRepository.ExternalAutoComplete();

            return Json(results);
        }

    }
}
