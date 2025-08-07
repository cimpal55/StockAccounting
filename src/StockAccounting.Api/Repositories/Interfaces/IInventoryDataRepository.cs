using StockAccounting.Core.Data.Models.Data.InventoryData;

namespace StockAccounting.Api.Repositories.Interfaces
{
    public interface IInventoryDataRepository
    {
        public Task<List<InventoryDataModel>> GetInventoryData();

        public Task<List<InventoryDataModel>> GetLatestInventoryData(DateTime lastSyncDateTime);

        public Task<List<InventoryDataModel>> GetCheckedInventoryData();

        public Task<int?> InsertInventoryDataAsync(InventoryDataModel inventoryData);

        public Task UpdateInventoryDataAsync(InventoryDataModel inventoryData);

        public Task<List<InventoryDataModel>> GetInprocessInventoryData();

        public Task<int?> GetInventoryDataIdByNameAsync(string name);
    }
}