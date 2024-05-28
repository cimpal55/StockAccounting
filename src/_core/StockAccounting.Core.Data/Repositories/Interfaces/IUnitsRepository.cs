using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IUnitsRepository
    {
        Task<IEnumerable<UnitModel>> GetUnitsAsync();
    }
}
