using Microsoft.AspNetCore.Mvc;
using Serilog;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Repositories;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Web.Constants;
using StockAccounting.Web.Extensions;
using StockAccounting.Web.Services.Interfaces;
using StockAccounting.Web.Utils;
using StockAccounting.Web.ViewModels;
using System.ComponentModel;

namespace StockAccounting.Web.Controllers
{
    public class ToolkitDataController : BaseController
    {
        private readonly IGenericRepository<ToolkitModel> _repository;
        private readonly IGenericRepository<ToolkitExternalModel> _toolkitExternalRepository;
        private readonly IToolkitRepository _toolkitRepository;
        private readonly IExternalDataRepository _externalDataRepository;
        private readonly IPaginationService _paginationService;
        public ToolkitDataController(IToolkitRepository toolkitRepository,
            IExternalDataRepository externalDataRepository,
            IGenericRepository<ToolkitModel> repository,
            IGenericRepository<ToolkitExternalModel> toolkitExternalRepository,
            IPaginationService paginationService)
        {
            _toolkitRepository = toolkitRepository;
            _externalDataRepository = externalDataRepository;
            _repository = repository;
            _toolkitExternalRepository = toolkitExternalRepository;
            _paginationService = paginationService;
        }

        public async Task<IActionResult> List(int pageId, string searchText)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            PaginatedData<ToolkitModel> data;
            IEnumerable<ToolkitExternalModel> toolkitExternalData;
            IEnumerable<ExternalDataModel> externalData;

            if (string.IsNullOrEmpty(searchText) || searchText == "dummyText")
                data = await _paginationService.PaginatedToolkits(pageId, _itemsPerPage);
            else
                data = await _paginationService.PaginatedSearchedToolkits(pageId, _itemsPerPage, searchText);

            toolkitExternalData = await _toolkitRepository.GetToolkitExternalDataAsync();
            externalData = await _externalDataRepository.GetExternalDataAsync();

            var toolkitViewModel = new ToolkitViewModel
            {
                ExternalDataModel = externalData.ToList(),
                ToolkitDataModel = data,
                ToolkitExternalDataModel = toolkitExternalData.ToList(),
                TotalPages = data.TotalPages,
                PageIndex = data.PageIndex,
                TotalData = data.TotalData,
                StartPage = pageId >= _pagesInRow ? pageId - 2 : 1,
                EndPage = pageId >= _pagesInRow && pageId < data.TotalPages ? pageId + 1 : data.TotalPages > _pagesInRow && pageId < data.TotalPages ? _pagesInRow : data.TotalPages,
                SearchText = searchText
            };

            toolkitViewModel.ToolkitExternalDataModel.Add(new ToolkitExternalModel());

            return View(toolkitViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> InsertToolkitData(ToolkitViewModel item)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                       .Where(y => y.Count > 0)
                       .ToList();
                this.AddAlertDanger($"{WebConstants.Error}. {errors}.");
                return BadRequest();
            }
            Log.Information("Insert Toolkit Data");
            Log.Debug("Model is: {item}", item);
            try
            {
                List<ToolkitExternalModel> insertExternalList = new();
                double totalQuantity = 0;
                int toolkitId;


                string barcode = await _toolkitRepository.ReturnToolkitBarcode();

                var model = new ToolkitModel
                {
                    Name = item.ToolkitName,
                    Barcode = barcode,
                    Description = item.Description,
                    TotalQuantity = totalQuantity,
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                };

                toolkitId = await _toolkitRepository.InsertToolkitWithIdentityAsync(model);
                Log.Debug("Toolkit created id is: {toolkitId}", toolkitId);

                foreach (var mod in item.ToolkitExternalDataModel)
                {
                    var externalToolkitModel = new ToolkitExternalModel
                    {
                        ExternalDataId = mod.ExternalDataId,
                        Quantity = mod.Quantity,
                        ToolkitId = toolkitId,
                        Created = DateTime.Now,
                        Updated = DateTime.Now
                    };

                    model.TotalQuantity += mod.Quantity;
                    insertExternalList.Add(externalToolkitModel);
                    Log.Debug("External toolkit model is: {externalToolkitModel}", externalToolkitModel);
                }

                foreach (var external in insertExternalList)
                    await _toolkitRepository.InsertExternalToolkit(external);

                await _repository.UpdateAsync(model);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            this.AddAlertSuccess(WebConstants.Success);
            return Json(new { redirectToUrl = Url.Action("List", "ToolkitData") });
        }

        public async Task<JsonResult> GetAutoCompleteExternalAsync()
        {
            var results = await _externalDataRepository.ExternalAutoComplete();

            return Json(results);
        }
    }
}
