using StockAccounting.Core.Android.Data;
using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Inventory.Models;
using StockAccounting.Inventory.Repositories.Interfaces;

namespace StockAccounting.Inventory.Repositories
{
    public partial class ScannedInventoryDataRepository : DatabaseContext, IScannedInventoryDataRepository
    {
        public async Task<IReadOnlyList<ScannedInventoryDataRecord>> GetFullListAsync() =>
            await GetAsyncConnection()
                .Table<ScannedInventoryDataRecord>()
                .ToArrayAsync();

        public async Task<IReadOnlyList<ScannedInventoryDataRecord>>
            GetDetListByDocIdAndByDetNameAsync(int docId, string name) =>
            await GetAsyncConnection()
                .QueryAsync<ScannedInventoryDataRecord>($"SELECT * FROM tblScannedData WHERE InventoryDataId = {docId} AND Barcode like '%{name}'")
                .ConfigureAwait(false);

        public async Task<IReadOnlyList<ScannedInventoryDataRecord>> GetDetListByDocIdAsync(int docId) =>
            await GetAsyncConnection()
                .Table<ScannedInventoryDataRecord>()
                .Where(x => x.InventoryDataId == docId)
                .OrderBy(x => x.Id)
                .ToArrayAsync();

        public async Task SaveNewAmountAsync(ScannedInventoryDataRecord item) =>
            await GetAsyncConnection()
                .UpdateAsync(item)
                .ConfigureAwait(false);

        public async Task InsertOrUpdateAsync(ScannedInventoryDataRecord item)
        {
            var conn = GetAsyncConnection();

            var existing = await conn.Table<ScannedInventoryDataRecord>()
                .Where(x => x.ExternalId == item.ExternalId && x.InventoryDataId == item.InventoryDataId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (existing != null)
            {
                existing.TotalCheckedQuantity += item.TotalCheckedQuantity;
                existing.Updated = DateTime.Now;

                await conn.UpdateAsync(existing).ConfigureAwait(false);
            }
            else
            {
                await conn.InsertAsync(item).ConfigureAwait(false);
            }
        }


        public async Task DeleteCheckedScannedData(int id) =>
            await GetAsyncConnection()
                .ExecuteAsync($"DELETE FROM tblScannedData WHERE InventoryDataId = {id}")
                .ConfigureAwait(false);
    }
}
