using LinqToDB;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.Core.Data.Repositories
{
    public class UnitsRepository : IUnitsRepository
    {
        private readonly AppDataConnection _conn;
        public UnitsRepository(AppDataConnection conn)
        {
            _conn = conn;
        }
        public async Task<IEnumerable<UnitModel>> GetUnitsAsync() =>
            await _conn
                .Units
                .ToListAsync();
    }
}
