using StockAccounting.Core.Data.Models.Data.EmployeeData;

namespace StockAccounting.Api.Repositories.Interfaces
{
    public interface IEmployeeDataRepository
    {
        Task<List<EmployeeDataModel>> GetEmployeeData();
    }
}
