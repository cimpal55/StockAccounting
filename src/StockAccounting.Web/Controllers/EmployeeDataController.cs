using Microsoft.AspNetCore.Mvc;
using StockAccounting.Web.Extensions;
using StockAccounting.Web.Services.Interfaces;
using StockAccounting.Web.Constants;
using StockAccounting.Core.Data.Enums;
using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.Web.Controllers
{
    public class EmployeeDataController : BaseController
    {
        private readonly IEmployeeDataRepository _repository;
        private readonly IPaginationService _paginationService;
        private readonly IFileExportService _fileExportService;
        private new int _itemsPerPage = 100;

        public EmployeeDataController
            (IEmployeeDataRepository employeeRepository,
            IPaginationService paginationService,
            IFileExportService fileExportService)
        {
            _repository = employeeRepository;
            _paginationService = paginationService;
            _fileExportService = fileExportService;
        }
        public async Task<IActionResult> List(int pageId, string searchText)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            var data = await _repository.GetEmployeesAsync();

            var employeeDataViewModel = new EmployeeDataViewModel
            {
                EmployeeModel = data,
            };

            return View(employeeDataViewModel);
        }

        public async Task<IActionResult> Details(int pageId, int id)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            var items = await _paginationService.PaginatedEmployeeDetails(pageId, _itemsPerPage, id);

            var data = new EmployeeDetailsViewModel
            {
                EmployeeDetailsModel = items,
                TotalPages = items.TotalPages,
                PageIndex = items.PageIndex,
                TotalData = items.TotalData,
                StartPage = pageId >= _pagesInRow ? pageId - 2 : 1,
                EndPage = pageId >= _pagesInRow && pageId < items.TotalPages ? pageId + 1 : items.TotalPages > _pagesInRow && pageId < items.TotalPages ? _pagesInRow : items.TotalPages,
            };

            return View(data);
        }

        public async Task<IActionResult> LeftQuantity(int pageId, int employeeId, int externalDataId)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            var items = await _paginationService.PaginatedEmployeeDetailLeftQuantity(pageId, _itemsPerPage,
                employeeId, externalDataId);

            var data = new EmployeeLeftQuantityViewModel()
            {
                EmployeeDetailLeftQuantityModel = items,
                TotalPages = items.TotalPages,
                PageIndex = items.PageIndex,
                TotalData = items.TotalData,
                StartPage = pageId >= _pagesInRow ? pageId - 2 : 1,
                EndPage = pageId >= _pagesInRow && pageId < items.TotalPages ? pageId + 1 : items.TotalPages > _pagesInRow && pageId < items.TotalPages ? _pagesInRow : items.TotalPages,
            };

            return View(data);
        }

        [HttpPost("ScannedReportExcel")]
        public async Task<IActionResult> ScannedReportExcel(int[] employeeList, FileExport mode)
        {
            try
            {
                var exportFileResponse = await _fileExportService.CreateScannedReportExcelFile(employeeList, mode);
                this.AddAlertSuccess(WebConstants.Success);
                return File(exportFileResponse.FileStream, exportFileResponse.ContentType, exportFileResponse.FileName);
            }
            catch (Exception e) {
                if (e.Source == "EPPlus")
                    this.AddAlertDanger($"{WebConstants.Error} Selected workers are missing stocks.");
                else
                    this.AddAlertDanger($"{WebConstants.Error} No employees have been selected.");
                return BadRequest();
            }
        }

        //[HttpGet("file")]
        //public async Task<FileStreamResult> ExportFile([FromQuery(Name = "fmt")] string format, [FromQuery(Name = "doctype")] string docType, [FromQuery(Name = "employeeId")] int employeeId, CancellationToken ct = default)
        //{
        //    var req = new EmployeeExportFileRequest
        //    {
        //        Format = format,
        //        DocType = docType
        //    };

        //    var res = await _fileExportService.Exp(req, employeeId, ct)
        //        .ConfigureAwait(false);

        //    return File(res.FileStream, res.ContentType, res.FileName);
        //}
    }
}
