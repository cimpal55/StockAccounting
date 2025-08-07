using Azure;
using Microsoft.AspNetCore.Mvc;
using StockAccounting.Core.Data.Enums;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Web.Constants;
using StockAccounting.Web.Extensions;
using StockAccounting.Web.Services;
using StockAccounting.Web.Services.Interfaces;
using System.Diagnostics;
using StockAccounting.Core.Data.Models.Data.StockData;
using static LinqToDB.Common.Configuration;

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

            PaginatedData<StockDataModel> data;

            if (string.IsNullOrEmpty(searchText) || searchText == "dummyText")
                data = await _paginationService.PaginatedStocks(pageId, _itemsPerPage);
            else
                data = await _paginationService.PaginatedSearchedStocks(pageId, _itemsPerPage, searchText);

            var stockDataViewModel = new StockDataViewModel
            {
                StockDataModel = data,
                TotalPages = data.TotalPages,
                PageIndex = data.PageIndex,
                TotalData = data.TotalData,
                StartPage = pageId >= _pagesInRow ? pageId - 2 : 1,
                EndPage = pageId >= _pagesInRow && pageId < data.TotalPages ? pageId + 1 : data.TotalPages > _pagesInRow && pageId < data.TotalPages ? _pagesInRow : data.TotalPages,

            };

            return View(stockDataViewModel);
        }

        public async Task<IActionResult> Details(int pageId, int id)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            var items = await _stockDataRepository.GetStockDetailsByIdAsync(id);

            var data = new StockEmployeesViewModel
            {
                StockEmployeesModel = items,
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

        public void RunManualSynchronization()
        {
            try
            {
                var fileName = "C:\\www\\StockAccounting\\Synchronization\\StockAccounting.Synchronization.exe";
                var fileInfoName = new FileInfo(fileName);

                if (FileHelper.IsFileInUseGeneric(fileInfoName))
                {
                    this.AddAlertDanger($"Synchronization already is in use.");
                }
                else
                {
                    Process process = new Process();

                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.FileName = fileName;

                    process.Start();
                    process.WaitForExit();

                    this.AddAlertSuccess(WebConstants.Success);
                }
            }
            catch (Exception e)
            {
                this.AddAlertDanger($"{WebConstants.Error}");
            }
        }
    }
}
