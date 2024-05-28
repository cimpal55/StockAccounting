using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Tools;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.Core.Data.Repositories
{
    public class EmployeeDataRepository : IEmployeeDataRepository
    {
        private readonly AppDataConnection _conn;

        public EmployeeDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public async Task<IEnumerable<EmployeeDataModel>> GetEmployeesAsync() =>
            await _conn
                .Employees
                .ToListAsync();

        public async Task<IEnumerable<EmployeeDataModel>> GetEmployeesSearchTextAsync(string searchText) =>
            await _conn
                .Employees
                .Where(x => string.Join(" ", x.Name, x.Surname, x.Code).Contains(searchText))
                .ToListAsync();

        public async Task<int> GetEmployeeIdByCode(string code) =>
            await _conn
                .Employees
                .Where(x => x.Code == code)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

        public async Task UpdateEmployeeAsync(EmployeeDataModel item) =>
            await _conn
                .Employees
                .Where(x => x.Id == item.Id)
                .Set(x => x.Name, item.Name)
                .Set(x => x.Surname, item.Surname)
                .UpdateAsync()
                .ConfigureAwait(false);

        public async Task<IEnumerable<StockDataModel>> GetEmployeeDetailsByIdAsync(int id)
        {
            var sql = @$"SELECT ex.Name, ex.Barcode, ex.ItemNumber, 
	                            ex.PluCode, SUM(ste.Quantity) as Quantity,
                                (em.Name + ' ' + em.Surname + ' ' + em.Code) as Employee,
                                em.Id as EmployeeId
	                         FROM TBL_Stock_Employees ste
	                         JOIN TBL_StockData st ON ste.StockDataID = st.ID
	                         JOIN TBL_ExternalData ex ON st.ExternalDataID = ex.ID
	                         JOIN TBL_CONF_Employees em ON ste.EmployeeID = em.ID
	                         WHERE em.ID = {id}
	                         GROUP BY ex.Name, ex.Barcode, ex.ItemNumber, ex.PluCode,
                                      (em.Name + ' ' + em.Surname + ' ' + em.Code), 
                                      em.Id
	                         HAVING SUM(ste.Quantity) != 0";

            return await _conn.QueryToListAsync<StockDataModel>(sql)
                .ConfigureAwait(false);
        }

    }
}
