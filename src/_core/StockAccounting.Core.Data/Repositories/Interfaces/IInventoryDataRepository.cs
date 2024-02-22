using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IInventoryDataRepository
    {
        Task<IEnumerable<InventoryDataModel>> GetInventoryDataAsync();

        Task<IEnumerable<InventoryDataModel>> GetInventoryDataSearchTextAsync(string searchText);

        Task UpdateInventoryDataAsync(InventoryDataBaseModel item);

        bool CheckIfDocumentHasScannedData(int inventoryId);
        Task<bool> CheckInventorySynchronizationAsync(int employeeId);
    }
}
