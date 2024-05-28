using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IInventoryDataRepository
    {
        Task<IEnumerable<InventoryDataModel>> GetInventoryDataAsync();
        Task<IEnumerable<InventoryDataModel>> GetInventoryDataSearchTextAsync(string searchText);
        Task UpdateInventoryDataAsync(InventoryDataBaseModel item);
        Task<int> InsertWithIdentityAsync(InventoryDataBaseModel item);
        bool CheckIfDocumentHasScannedData(int inventoryId);
        Task<int> ReturnDocumentIdIfExists(InventoryDataBaseModel item);
        Task<List<ScannedDataBaseModel>> ReturnDocumentScannedData(int inventoryId);
        Task<List<StockEmployeesBaseModel>> ReturnStockEmployeesByDocumentNumber(ScannedDataBaseModel data);
        Task<bool> CheckInventorySynchronizationAsync(int employeeId);
        Task<bool> ReturnInventorySynchronizationAsync(int documentId);
    }
}
