using StockAccounting.Core.Data.Models.Data.InventoryData;
using StockAccounting.Core.Data.Models.Data.ScannedInventoryData;

namespace StockAccounting.Api.Repositories.Interfaces
{
    public interface IScannedInventoryDataRepository
    {
        //public Task<int?> InsertInventoryDataAsync(InventoryDataModel inventoryData);

        public Task InsertScannedDataAsync(IEnumerable<ScannedInventoryDataModel> scannedData, int id);

        public Task UpdateScannedDataAsync(IEnumerable<ScannedInventoryDataModel> scannedData, int id);

        public Task<List<ScannedInventoryDataModel>> GetScannedData();

        public Task<List<ScannedInventoryDataModel>> GetLatestScannedData(IEnumerable<InventoryDataModel> data);
        
        Task<decimal> GetDetailQuantityByEmployeeIdAsync(int employeeId, int detailId);

        Task<IEnumerable<ScannedInventoryDataModel>> GetDetailsByEmployeeIdAsync(int id);
    }
}