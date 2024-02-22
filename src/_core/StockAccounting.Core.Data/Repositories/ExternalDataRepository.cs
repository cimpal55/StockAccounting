using LinqToDB;
using LinqToDB.Data;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using System.Security.Cryptography;

namespace StockAccounting.Core.Data.Repositories
{
    public class ExternalDataRepository : IExternalDataRepository
    {
        private readonly AppDataConnection _conn;
        public ExternalDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public async Task<IEnumerable<ExternalDataModel>> GetExternalDataAsync()
        {
            var sql =
                "SELECT TOP 100 PERCENT ROW_NUMBER() OVER (ORDER BY ed.ID) AS RowId, " +
                "ed.ID, ed.Plucode, ed.Barcode, ed.ItemNumber, ed.Name, ed.Unit " +
                "FROM TBL_ExternalData as ed " +
                "GROUP BY ed.ID, ed.PluCode, ed.ItemNumber, ed.Name, ed.Barcode, " +
                "ed.Unit";

            return await _conn.QueryToListAsync<ExternalDataModel>(sql)
                .ConfigureAwait(false);
        }
        public async Task<IEnumerable<ExternalDataModel>> GetExternalDataSearchTextAsync(string searchText)
        {
            var sql = "SELECT TOP 100 PERCENT ROW_NUMBER() OVER (ORDER BY ed.ID) AS RowId, " +
                      "ed.ID, ed.PluCode, ed.ItemNumber, ed.Name, ed.Barcode, ed.Unit " +
                      "FROM TBL_ExternalData as ed " +
                      "WHERE ed.Barcode LIKE '%" + searchText + "%' OR ed.PluCode LIKE '%" + searchText + "%' " +
                      "OR ed.Name LIKE '%" + searchText + "%' " +
                      "OR ed.ItemNumber LIKE '%" + searchText + "%' " +
                      "GROUP BY ed.ID, ed.PluCode, ed.ItemNumber, ed.Name, ed.Barcode, " +
                      "ed.Unit";

            return await _conn.QueryToListAsync<ExternalDataModel>(sql)
                .ConfigureAwait(false);
        }

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

        public List<ScannedDataModel> GetFinishedListForHtml(List<ScannedDataBaseModel> stocksList)
        {
            List<ScannedDataModel> stocks = new();

            foreach (var item in stocksList)
            {
                var record = GetExternalDataById(item.ExternalDataId);
                ScannedDataModel stock = new()
                {
                    Name = record.Name,
                    ItemNumber = record.ItemNumber,
                    PluCode = record.PluCode,
                    Quantity = item.Quantity,
                    Created = item.Created,
                };
                stocks.Add(stock);
            }

            return stocks;
        }

        public async Task<IEnumerable<AutocompleteModel>> ExternalAutoComplete()
        {
            var query = (from c in _conn.ExternalData
                         select new AutocompleteModel
                         {
                             Id = c.Id,
                             Text = string.Join(" ", c.Name, c.ItemNumber, c.PluCode),
                         });

            return await query.ToListAsync();
        }
    }
}
