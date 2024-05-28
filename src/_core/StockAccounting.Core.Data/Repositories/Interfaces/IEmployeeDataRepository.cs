using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IEmployeeDataRepository
    {
        Task<IEnumerable<EmployeeDataModel>> GetEmployeesAsync();

        Task<IEnumerable<EmployeeDataModel>> GetEmployeesSearchTextAsync(string searchText);

        Task<int> GetEmployeeIdByCode(string code);

        Task UpdateEmployeeAsync(EmployeeDataModel item);

        Task<IEnumerable<StockDataModel>> GetEmployeeDetailsByIdAsync(int id);
    }
}
