using LinqToDB;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Api.Repositories
{
    public class ToolkitDataRepository : IToolkitDataRepository
    {
        private readonly AppDataConnection _conn;
        public ToolkitDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }
        public async Task<List<ToolkitModel>> GetToolkitData() =>
            await _conn
                .Toolkits
                .ToListAsync();

        public async Task<List<ToolkitExternalModel>> GetToolkitExternalDataByToolkitIdAsync(int id)
        {
            var list = await _conn
                .ToolkitExternal
                .Where(x => x.ToolkitId == id)
                .ToListAsync();

            return list;
        }
    }
}

