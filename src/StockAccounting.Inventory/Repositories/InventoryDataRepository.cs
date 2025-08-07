using StockAccounting.Core.Android.Data;
using StockAccounting.Core.Android.Enums;
using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Inventory.Repositories.Interfaces;

namespace StockAccounting.Inventory.Repositories
{
    public sealed class InventoryDataRepository : DatabaseContext, IInventoryDataRepository
    {
        public async Task<IReadOnlyList<InventoryDataRecord>> GetFullListAsync() =>
            await GetAsyncConnection()
                .Table<InventoryDataRecord>()
                .ToArrayAsync();
     
        public async Task<IReadOnlyList<InventoryDataRecord>> GetListAsync() =>
            await GetAsyncConnection()
                .Table<InventoryDataRecord>()
                .Where(x => x.Status != "Checked")
                .ToArrayAsync();

        public async Task<IReadOnlyList<InventoryDataRecord>> GetListByNameAsync(string name) =>
            await GetAsyncConnection()
                .Table<InventoryDataRecord>()
                .Where(x => x.Name.ToLower().Contains(name.ToLower()))
                .ToArrayAsync();

        public async Task<InventoryDataRecord> GetInventoryDataByIdAsync(int id) =>
            await GetAsyncConnection()
                .Table<InventoryDataRecord>()
                .Where(x => x.ExternalId == id)
                .FirstOrDefaultAsync();

        public async Task UpdateInventoryDocStatus(int id, string status) =>
            await GetAsyncConnection()
                  .ExecuteAsync($"UPDATE tblInventoryData SET Status = '{status}' WHERE ExternalId = {id}")
                  .ConfigureAwait(false);

        public async Task DeleteCheckedInventoryData(int id) =>
            await GetAsyncConnection()
                .ExecuteAsync($"DELETE FROM tblInventoryData WHERE ExternalId = {id}")
                .ConfigureAwait(false);

    }
}
