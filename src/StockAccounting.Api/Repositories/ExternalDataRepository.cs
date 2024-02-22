using LinqToDB;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;

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
                .FirstAsync();

        public ExternalDataModel GetExternalDataById(int externalDataId)
        {
            var query = from c in _conn.ExternalData
                        where c.Id == externalDataId
                        select new ExternalDataModel
                        {
                            Name = c.Name,
                            ItemNumber = c.ItemNumber,
                            PluCode = c.PluCode
                        };

            return query.FirstOrDefault() ?? new();
        }
    }
}
