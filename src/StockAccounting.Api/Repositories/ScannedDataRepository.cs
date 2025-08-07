using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.Data.ScannedData;

namespace StockAccounting.Api.Repositories
{
    public class ScannedDataRepository : IScannedDataRepository
    {
        private readonly AppDataConnection _conn;
        public ScannedDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }
        public async Task<List<ScannedDataBaseModel>> GetScannedData() =>
            await _conn
                .ScannedData
                .ToListAsync();

        public async Task<int> InsertScannedData(ScannedDataBaseModel scannedData)
        {
            var record = new ScannedDataBaseModel
            {
                ExternalDataId = scannedData.ExternalDataId,
                DocumentDataId = scannedData.DocumentDataId,
                Quantity = scannedData.Quantity,
                Created = DateTime.Now
            };

            return await _conn
                .InsertAsync(record);
        }

        public async Task<IEnumerable<ScannedDataBaseModel>> GetScannedDataByIsSynchronizationAsync(int id, int externalDataId) =>
            await _conn
                .ScannedData
                .Join(_conn.DocumentData, x => x.DocumentDataId, iv => iv.Id, (x, iv) => new { x, iv })
                .Join(_conn.ExternalData, v => v.x.ExternalDataId, ex => ex.Id, (v, ex) => new { v, ex })
                .Where(x => x.v.iv.Employee1Id == id && x.v.iv.IsSynchronization == true && x.ex.Id == externalDataId)
                .Select(x => x.v.x)
                .ToListAsync();
    }
}