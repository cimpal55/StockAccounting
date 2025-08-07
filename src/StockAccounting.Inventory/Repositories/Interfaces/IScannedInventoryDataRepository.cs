 using StockAccounting.Core.Android.Models.Data;
 using StockAccounting.Inventory.Models;

 namespace StockAccounting.Inventory.Repositories.Interfaces
{
    public interface IScannedInventoryDataRepository
    {
        Task<IReadOnlyList<ScannedInventoryDataRecord>> GetFullListAsync();

        Task<IReadOnlyList<ScannedInventoryDataRecord>> GetDetListByDocIdAndByDetNameAsync(int docId, string name);

        Task<IReadOnlyList<ScannedInventoryDataRecord>> GetDetListByDocIdAsync(int docId);

        Task SaveNewAmountAsync(ScannedInventoryDataRecord item);

        Task InsertOrUpdateAsync(ScannedInventoryDataRecord item);

        Task DeleteCheckedScannedData(int id);
    }
}
