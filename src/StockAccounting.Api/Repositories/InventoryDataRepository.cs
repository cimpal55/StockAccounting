using LinqToDB;
using Serilog;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Android.Enums;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Enums;
using StockAccounting.Core.Data.Models.Data.InventoryData;

namespace StockAccounting.Api.Repositories
{
    public class InventoryDataRepository : IInventoryDataRepository
    {
        private readonly AppDataConnection _conn;

        public InventoryDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public async Task<List<InventoryDataModel>> GetInventoryData() =>
            await _conn
                .InventoryData
                .ToListAsync();

        public async Task<List<InventoryDataModel>> GetLatestInventoryData(DateTime lastSyncDateTime) =>
            await _conn
                .InventoryData
                .Where(x => x.Updated > lastSyncDateTime)
                .ToListAsync();

        public async Task<List<InventoryDataModel>> GetCheckedInventoryData() =>
            await _conn
                .InventoryData
                .Where(x => x.Status == InventoryStatus.Checked.ToString())
                .ToListAsync();

        public async Task<int?> GetInventoryDataIdByNameAsync(string name) =>
            await _conn
                .InventoryData
                .Where(x => x.Name == name)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

        public async Task<int?> InsertInventoryDataAsync(InventoryDataModel inventoryData)
        {
            int docId;

            try
            {
                await _conn.BeginTransactionAsync();

                docId = await _conn.InventoryData
                    .InsertWithInt32IdentityAsync(() => new InventoryDataModel
                    {
                        Name = inventoryData.Name,
                        Employee1CheckerId = inventoryData.Employee1CheckerId,
                        Employee2CheckerId = inventoryData.Employee2CheckerId,
                        ScannedEmployeeId = inventoryData.ScannedEmployeeId,
                        Status = InventoryStatus.InProcess.ToString(),
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        Finished = null,
                    });

                await _conn.CommitTransactionAsync();

            }
            catch (Exception ex)
            {
                await _conn.RollbackTransactionAsync();
                Log.Information("API_InsertInventoryDataAsync {message}", ex.Message);
                throw;
            }

            return docId;
        }

        public async Task UpdateInventoryDataAsync(InventoryDataModel inventoryData)
        {
            try
            {
                await _conn.BeginTransactionAsync();

                DateTime? finished = inventoryData.Status == InventoryStatus.Checked.ToString() ? DateTime.Now : null;

                await _conn.InventoryData
                    .Where(x => x.Id == inventoryData.Id)
                    .Set(x => x.Employee1CheckerId, inventoryData.Employee1CheckerId)
                    .Set(x => x.Employee2CheckerId, inventoryData.Employee2CheckerId)
                    .Set(x => x.ScannedEmployeeId, inventoryData.ScannedEmployeeId)
                    .Set(x => x.Status, inventoryData.Status)
                    .Set(x => x.Updated, DateTime.Now)
                    .Set(x => x.Finished, finished)
                    .UpdateAsync();

                await _conn.CommitTransactionAsync();

            }
            catch (Exception ex)
            {
                await _conn.RollbackTransactionAsync();
                Log.Information("API_UpdateInventoryDataAsync {message}", ex.Message);
                throw;
            }
        }

        public async Task<List<InventoryDataModel>> GetInprocessInventoryData() =>
            await _conn
                .InventoryData
                .Where(x => x.Status == InventoryStatus.InProcess.ToString())
                .ToListAsync();

    }
}
