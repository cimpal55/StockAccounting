using Microsoft.AspNetCore.Mvc;
using Serilog;
using StockAccounting.Web.Constants;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Web.Services.Interfaces;
using StockAccounting.Web.Utils;
using StockAccounting.Web.ViewModels;
using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.Web.Controllers
{
    public class ExternalDataController : BaseController
    {
        private readonly IExternalDataRepository _repository;
        private readonly IPaginationService _paginationService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IFileImportService _fileImportService;
        public ExternalDataController(
            IExternalDataRepository repository,
            IPaginationService paginationService,
            IFileUploadService fileUploadService,
            IFileImportService fileImportService)
        {
            _repository = repository;
            _paginationService = paginationService;
            _fileUploadService = fileUploadService;
            _fileImportService = fileImportService;
        }
        public async Task<IActionResult> List(int id, string searchText)
        {
            if (id == 0) id = 1;
            if (id < 0)
                return BadRequest();

            PaginatedData<ExternalDataModel> data;

            if (string.IsNullOrEmpty(searchText) || searchText == "dummyText")
                data = await _paginationService.PaginatedProducts(id, _itemsPerPage);
            else
                data = await _paginationService.PaginatedSearchedProducts(id, _itemsPerPage, searchText);

            var externalDataViewModel = new ExternalDataViewModel
            {
                ExternalDataModel = data,
                TotalPages = data.TotalPages,
                PageIndex = data.PageIndex,
                TotalData = data.TotalData,
                StartPage = id >= _pagesInRow ? id - 2 : 1,
                EndPage = id >= _pagesInRow && id < data.TotalPages ? id + 1 : data.TotalPages > _pagesInRow && id < data.TotalPages ? _pagesInRow : data.TotalPages,
                SearchText = searchText
            };

            return View(externalDataViewModel);
        }

        [HttpPost("UploadNetsuiteDataFile")]
        public async Task<IActionResult> UploadNetsuiteDataFile(IFormFile file)
        {
            try
            {
                if (await _fileUploadService.UploadFile(file))
                {
                    var newFilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, WebConstants.UploadedDir));

                    using var reader = new StreamReader(Path.Combine(newFilePath, file.FileName));

                    await _fileImportService.ImportNetSuiteCsvData(reader);
                }
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
            }

            return RedirectToAction("List");
        }

    }
}
