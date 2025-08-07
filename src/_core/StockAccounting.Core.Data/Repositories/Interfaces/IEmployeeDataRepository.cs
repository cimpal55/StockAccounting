using StockAccounting.Core.Data.Models.Data.EmployeeData;
using StockAccounting.Core.Data.Models.Data.StockData;
using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IEmployeeDataRepository
    {
        Task<IEnumerable<EmployeeDataModel>> GetEmployeesAsync();

        IQueryable<EmployeeDataModel> GetEmployeesQueryable();

        IQueryable<EmployeeDataModel> GetEmployeesSearchTextQueryable(string searchText);

        Task<int> GetEmployeeIdByCode(string code);

        string GetEmployeeEmailByCode(string code);

        Task UpdateEmployeeAsync(EmployeeDataModel item);

        Task<IEnumerable<StockDataModel>> GetEmployeeDetailsByIdAsync(int id);

        IQueryable<EmployeeDetailsListModel> GetEmployeeDetailsByIdQueryable(int id);

        IQueryable<EmployeeDetailLeftQuantityListModel> GetEmployeeDetailLeftQuantityByIdQueryable(int employeeId,
            int externalDataId);
    }
}
