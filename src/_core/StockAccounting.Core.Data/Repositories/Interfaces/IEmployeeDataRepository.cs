using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IEmployeeDataRepository
    {
        Task<IEnumerable<EmployeeDataModel>> GetEmployeesAsync();

        Task<IEnumerable<EmployeeDataModel>> GetEmployeesSearchTextAsync(string searchText);

        string GetEmployeeFullNameById(int employeeId);

        Task UpdateEmployeeAsync(EmployeeDataModel item);

        Task<IEnumerable<ScannedDataModel>> GetEmployeeDetailsByIdAsync(int id);
    }
}
