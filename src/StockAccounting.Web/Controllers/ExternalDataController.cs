using Microsoft.AspNetCore.Mvc;
using Serilog;
using StockAccounting.Web.Constants;
using StockAccounting.Web.Services.Interfaces;
using StockAccounting.Web.Extensions;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.Unit;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.Web.Controllers
{
    public class ExternalDataController : BaseController
    {
        private readonly ILogger<ExternalDataController> _logger;
        private readonly IExternalDataRepository _repository;
        private readonly IGenericRepository<ExternalDataModel> _gRepository;
        private readonly IUnitsRepository _unitsRepository;
        private readonly IPaginationService _paginationService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IFileImportService _fileImportService;
        public ExternalDataController(
            IExternalDataRepository repository,
            IPaginationService paginationService,
            IFileUploadService fileUploadService,
            IFileImportService fileImportService,
            IUnitsRepository unitsRepository,
            IGenericRepository<ExternalDataModel> gRepository)
        {
            _repository = repository;
            _paginationService = paginationService;
            _fileUploadService = fileUploadService;
            _fileImportService = fileImportService;
            _unitsRepository = unitsRepository;
            _gRepository = gRepository;
        }

        //[HttpPost]
        //public async Task<IActionResult> GetExternalData([FromBody] DtParameters dtParameters)
        //{

        //    var searchBy = dtParameters.Search?.Value;

        //    // if we have an empty search then just order the results by Id ascending
        //    var orderCriteria = "Id";
        //    var orderAscendingDirection = true;

        //    if (dtParameters.Order != null)
        //    {
        //        orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
        //        orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
        //    }

        //    var result = await _repository.GetExternalDataAsync();
        //    var totalResultsCount = result.Count();

        //    if (!string.IsNullOrEmpty(searchBy))
        //    {
        //        result = await _repository.GetExternalDataSearchTextAsync(searchBy);
        //    }

        //    result = orderAscendingDirection ? result.AsQueryable().OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.AsQueryable().OrderByDynamic(orderCriteria, DtOrderDir.Desc);

        //    // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
        //    var filteredResultsCount = result.Count();

        //    return Json(new DtResult<ExternalDataModel>
        //    {
        //        Draw = dtParameters.Draw,
        //        RecordsTotal = totalResultsCount,
        //        RecordsFiltered = filteredResultsCount,
        //        Data = result
        //            .Skip(dtParameters.Start)
        //            .Take(dtParameters.Length)
        //            .ToList()
        //    });
        //}

        public async Task<IActionResult> List(int pageId, string searchText)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            PaginatedData<ExternalDataModel> data;
            IEnumerable<UnitModel> unitList;

            if (string.IsNullOrEmpty(searchText) || searchText == "dummyText")
                data = await _paginationService.PaginatedProducts(pageId, _itemsPerPage);
            else
                data = await _paginationService.PaginatedSearchedProducts(pageId, _itemsPerPage, searchText);
                
            unitList = await _unitsRepository.GetUnitsAsync();

            var externalDataViewModel = new ExternalDataViewModel
            {
                ExternalDataModel = data,
                UnitDataModel = unitList,
                TotalPages = data.TotalPages,
                PageIndex = data.PageIndex,
                TotalData = data.TotalData,
                StartPage = pageId >= _pagesInRow ? pageId - 2 : 1,
                EndPage = pageId >= _pagesInRow && pageId < data.TotalPages ? pageId + 1 : data.TotalPages > _pagesInRow && pageId < data.TotalPages ? _pagesInRow : data.TotalPages,
                SearchText = searchText
            };

            
            ViewBag.ID = pageId;

            return View(externalDataViewModel);
        }

        [HttpPost("InsertExternalData")]
        public async Task<ActionResult> InsertExternalData(ExternalDataViewModel item)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                       .Where(y => y.Count > 0)
                       .ToList();
                this.AddAlertDanger($"{WebConstants.Error}. {errors}.");
                return BadRequest();
            }

            var pages = await _paginationService.PaginatedProducts(1, _itemsPerPage);
            var pageId = pages.TotalData % _itemsPerPage == 0 ? pages.TotalPages + 1 : pages.TotalPages;

            var externalDataModel = new ExternalDataModel
            {
                Id = item.Id,
                Barcode = item.Barcode,
                ItemNumber = item.ItemNumber,
                Name = item.Name,
                PluCode = item.PluCode,
                Unit = item.UnitName,
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            try
            {
                Log.Information("WEB_InsertExternalData {@Data}", externalDataModel);
                Log.CloseAndFlush();
                await _gRepository.InsertAsync(externalDataModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            this.AddAlertSuccess(WebConstants.Success);
            return RedirectToAction("List", new { pageId = $"{pageId}" });
        }

        [HttpPost("UpdateExternalData")]
        public async Task<ActionResult> UpdateExternalData(ExternalDataViewModel item)
        {
            if (!ModelState.IsValid)
            {
                this.AddAlertDanger("Error: Document is not updated!");
                return BadRequest();
            }

            var externalDataModel = new ExternalDataModel
            {
                Id = item.Id,
                Name = item.Name,
                Barcode = item.Barcode,
                PluCode = item.PluCode,
                ItemNumber = item.ItemNumber,
                Unit = item.UnitName,
            };

            try
            {
                Log.Information("WEB_UpdateExternalData {@Data}", externalDataModel);
                Log.CloseAndFlush();
                await _repository.UpdateExternalDataAsync(externalDataModel);
            }
            catch (Exception e)
            {
                this.AddAlertDanger($"{WebConstants.Error} {e.Message}.");
                _logger.LogError(e.Message);
            }

            this.AddAlertSuccess(WebConstants.Success);
            return RedirectToAction("List", new { pageId = $"{item.PageId}" });
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
