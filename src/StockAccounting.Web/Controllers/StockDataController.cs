using Azure;
using Microsoft.AspNetCore.Mvc;
using StockAccounting.Core.Data;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Web.Constants;
using StockAccounting.Web.Extensions;
using StockAccounting.Web.Services;
using StockAccounting.Web.Services.Interfaces;
using StockAccounting.Web.ViewModels;

namespace StockAccounting.Web.Controllers
{
    public class StockDataController : BaseController
    {
        private readonly IStockDataRepository _stockDataRepository;
        private readonly IPaginationService _paginationService;
        private readonly IFileExportService _fileExportService;
        public StockDataController(IStockDataRepository stockDataRepository,
            IPaginationService paginationService,
            IFileExportService fileExportService)
        {
            _stockDataRepository = stockDataRepository;
            _paginationService = paginationService;
            _fileExportService = fileExportService;
        }
        public async Task<IActionResult> List(int pageId, string searchText)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            var data = await _stockDataRepository.GetStockDataAsync();

            var stockDataViewModel = new StockDataViewModel
            {
                StockDataModel = data,
            };

            return View(stockDataViewModel);
        }

        public async Task<IActionResult> Details(int pageId, int id)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            var items = await _paginationService.PaginatedStockDetails(pageId, _itemsPerPage, id);

            var data = new StockEmployeesViewModel
            {
                StockEmployeesModel = items,
                TotalPages = items.TotalPages,
                PageIndex = items.PageIndex,
                TotalData = items.TotalData,
                StartPage = pageId >= _pagesInRow ? pageId - 2 : 1,
                EndPage = pageId >= _pagesInRow && pageId < items.TotalPages ? pageId + 1 : items.TotalPages > _pagesInRow && pageId < items.TotalPages ? _pagesInRow : items.TotalPages,
            };

            return View(data);
        }

        public async Task<IActionResult> LeftQuantity(int stockDataId)
        {
            var items = await _stockDataRepository.GetStockLeftQuantity(stockDataId);

            var data = new StockEmployeesViewModel
            {
                StockEmployeesModel = items,
            };

            return View(data);
        }

        [HttpPost("StockReportExcel")]
        public async Task<IActionResult> StockReportExcel(int[] stocksList, FileExport mode)
        {
            try
            {
                var exportFileResponse = await _fileExportService.CreateStockReportExcelFile(stocksList, mode);
                this.AddAlertSuccess(WebConstants.Success);
                return File(exportFileResponse.FileStream, exportFileResponse.ContentType, exportFileResponse.FileName);
            }
            catch (Exception e)
            {
                if (e.Source == "EPPlus")
                    this.AddAlertDanger($"{WebConstants.Error} Selected stocks are missing details.");
                else
                    this.AddAlertDanger($"{WebConstants.Error} No stocks have been selected.");
                return BadRequest();
            }
        }
    }
}
