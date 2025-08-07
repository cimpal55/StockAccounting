using Microsoft.AspNetCore.Mvc;
using StockAccounting.Core.Data.Models.Data.EmployeeData;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Web.Repositories.Interfaces;
using StockAccounting.Web.Services.Interfaces;

namespace StockAccounting.Web.Controllers
{
    public class InventoryDataController : BaseController
    {
        private readonly IPaginationService _paginationService;
        private readonly IDataRepository _repository;

        public InventoryDataController(
            IPaginationService paginationService,
            IDataRepository repository)
        {
            _paginationService = paginationService;
            _repository = repository;
        }
        public async Task<IActionResult> List(int pageId, string searchText, string sortBy)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            PaginatedData<InventoryListModel> data;

            if (string.IsNullOrEmpty(searchText) || searchText == "dummyText")
                data = await _paginationService.PaginatedInventory(pageId, _itemsPerPage);
            else
                data = await _paginationService.PaginatedSearchedInventory(pageId, _itemsPerPage, searchText);

            var inventoryDataViewModel = new InventoryDataViewModel
            {
                InventoryDataModel = data,
                TotalPages = data.TotalPages,
                PageIndex = data.PageIndex,
                TotalData = data.TotalData,
                StartPage = pageId >= _pagesInRow ? pageId - 2 : 1,
                EndPage = pageId >= _pagesInRow && pageId < data.TotalPages ? pageId + 1 : data.TotalPages > _pagesInRow && pageId < data.TotalPages ? _pagesInRow : data.TotalPages,
                SearchText = searchText
            };

            return View(inventoryDataViewModel);
        }

        public async Task<IActionResult> Details(int pageId, string searchText, string sortBy, int id)
        {
            if (pageId == 0) pageId = 1;
            if (pageId < 0)
                return BadRequest();

            IEnumerable<InventoryDetailsListModel> data;

            if (string.IsNullOrEmpty(searchText) || searchText == "dummyText")
                data = _repository.Inventory.GetInventoryDetailsQueryable(id);
            else
                data = _repository.Inventory.GetInventoryDetailsQueryable(id, searchText);

            var inventoryDataViewModel = new InventoryDetailsViewModel()
            {
                InventoryDetailsModel = data,
                SearchText = searchText
            };

            return View(inventoryDataViewModel);
        }
    }
}
