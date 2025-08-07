using LinqToDB;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data.ExternalData;

namespace StockAccounting.Api.Repositories
{
    public class ExternalDataRepository : IExternalDataRepository
    {
        private readonly AppDataConnection _conn;

        public ExternalDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }
        public async Task<List<ExternalDataModel>> GetExternalData() =>
            await _conn
                .ExternalData
                .ToListAsync();

        public async Task<ExternalDataModel> GetExternalDataByBarcode(string barcode) =>
            await _conn
                .ExternalData
                .Where(x => x.Barcode == barcode)
                .FirstOrDefaultAsync();

        public async Task<ExternalDataModel> GetExternalDataById(int externalDataId) =>
            await _conn
                .ExternalData
                .Where(x => x.Id == externalDataId)
                .FirstOrDefaultAsync();
    }
}
