using LinqToDB;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data.Toolkit;
using StockAccounting.Core.Data.Models.Data.ToolkitExternal;
using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.Core.Data.Repositories
{
    public class ToolkitRepository : IToolkitRepository
    {
        private readonly AppDataConnection _conn;
        public ToolkitRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public IQueryable<ToolkitModel> GetToolkitDataQueryable()
        {
            return _conn.Toolkits;
        }

        public IQueryable<ToolkitModel> GetToolkitDataBySearchTextQueryable(string searchText)
        {
            return _conn.Toolkits
                .Where(x => x.Name.Contains(searchText) || x.Description.Contains(searchText));
        }


        public async Task<IEnumerable<ToolkitExternalModel>> GetToolkitExternalDataAsync()
        {
            var query = from tk in _conn.ToolkitExternal
                        join ex in _conn.ExternalData on tk.ExternalDataId equals ex.Id
                        select new ToolkitExternalModel
                        {
                            ExternalDataId = tk.ExternalDataId,
                            ExternalDataName = ex.Name,
                            Quantity = tk.Quantity,
                            ToolkitId = tk.ToolkitId,
                            Created = tk.Created,
                            Updated = tk.Updated,
                        };

            return await query.ToListAsync();
        }

        public async Task<string> ReturnToolkitBarcode()
        {
            bool checkIfToolkitAny = _conn
                                .Toolkits
                                .Any();
            string barcode;

            if (checkIfToolkitAny)
            {
                barcode = await _conn.Toolkits
                               .OrderByDescending(x => x.Barcode)
                               .Take(1)
                               .Select(x => x.Barcode)
                               .SingleOrDefaultAsync();

                barcode = Convert.ToString(Convert.ToInt64(barcode) + 1);
            }
            else
            {
                barcode = "900000000000";
            }

            return barcode;
        }

        public async Task<int> InsertToolkitWithIdentityAsync(ToolkitModel model) =>
            await _conn
                .InsertWithInt32IdentityAsync(model);

        public async Task InsertExternalToolkit(ToolkitExternalModel model)
        {
            await _conn
                .InsertAsync(model);

            var toolkit = await _conn
                            .Toolkits
                            .FirstOrDefaultAsync(x => x.Id == model.ToolkitId);

            toolkit.TotalQuantity += model.Quantity;

            await _conn
                    .UpdateAsync(toolkit);
        }
    }
}
