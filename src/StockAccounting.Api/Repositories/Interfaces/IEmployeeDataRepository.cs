using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Api.Repositories.Interfaces
{
    public interface IEmployeeDataRepository
    {
        Task<List<EmployeeDataModel>> GetEmployeeData();
    }
}
