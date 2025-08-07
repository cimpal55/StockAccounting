using StockAccounting.Core.Data.Models.Data.Unit;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IUnitsRepository
    {
        Task<IEnumerable<UnitModel>> GetUnitsAsync();
    }
}
