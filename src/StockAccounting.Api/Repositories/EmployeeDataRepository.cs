using LinqToDB;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data.EmployeeData;

namespace StockAccounting.Api.Repositories
{
    public class EmployeeDataRepository : IEmployeeDataRepository
    {
        private readonly AppDataConnection _conn;

        public EmployeeDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public async Task<List<EmployeeDataModel>> GetEmployeeData()
        {
            var employees = await _conn
                .Employees
                .ToListAsync();

            return employees;
        }
    }
}
