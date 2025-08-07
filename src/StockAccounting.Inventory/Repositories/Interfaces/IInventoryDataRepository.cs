using StockAccounting.Core.Android.Models.Data;

namespace StockAccounting.Inventory.Repositories.Interfaces
{
    public interface IInventoryDataRepository
    {
        Task<IReadOnlyList<InventoryDataRecord>> GetFullListAsync();

        Task<IReadOnlyList<InventoryDataRecord>> GetListAsync();

        Task<IReadOnlyList<InventoryDataRecord>> GetListByNameAsync(string name);

        Task<InventoryDataRecord> GetInventoryDataByIdAsync(int id);

        Task UpdateInventoryDocStatus(int id, string status);

        Task DeleteCheckedInventoryData(int id);
    }
}