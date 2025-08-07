using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        IQueryable<InventoryListModel> GetInventoryDataQueryable();
        IQueryable<InventoryListModel> GetInventoryDataSearchTextQueryable(string searchText);
        IQueryable<InventoryDetailsListModel> GetInventoryDetailsQueryable(int inventoryId, string searchText = null);
    }
}
