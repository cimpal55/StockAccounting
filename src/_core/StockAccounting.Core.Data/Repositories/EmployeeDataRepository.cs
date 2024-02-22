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

        public string GetEmployeeFullNameById(int employeeId) =>
            _conn
                .Employees
                .Where(x => x.Id == employeeId)
                .Select(x => string.Join(" ", x.Name, x.Surname, x.Code))
                .FirstOrDefault() ?? "";

        public async Task UpdateEmployeeAsync(EmployeeDataModel item) =>
            await _conn
                .Employees
                .Where(x => x.Id == item.Id)
                .Set(x => x.Name, item.Name)
                .Set(x => x.Surname, item.Surname)
                .UpdateAsync()
                .ConfigureAwait(false);

        public async Task<IEnumerable<ScannedDataModel>> GetEmployeeDetailsByIdAsync(int id)
        {
            var sql = @$"SELECT t1.Name, t1.ItemNumber, t1.Plucode, (t1.LeftQuantity - t2.ReturnQuantity) AS Quantity
                         FROM
                         (SELECT ex.Name as Name, ex.ItemNumber, ex.PluCode, SUM(sc.Quantity) as LeftQuantity
	                                                     FROM TBL_InventoryData iv
	                                                     JOIN TBL_ScannedData sc ON iv.ID = sc.InventoryDataID
	                                                     JOIN TBL_ExternalData ex on sc.ExternalDataId = ex.ID
	                                                     WHERE iv.IsSynchronization = 1 AND iv.Employee2ID = {id}
						   	                            GROUP BY ex.Name, ex.ItemNumber, ex.PluCode) as t1
                         LEFT JOIN
                         (SELECT exn.Name as Name, SUM(scn.Quantity) as ReturnQuantity
                                         FROM TBL_ScannedData scn
                                             JOIN TBL_InventoryData inv on inv.ID = scn.InventoryDataID
                                             JOIN TBL_ExternalData exn on scn.ExternalDataId = exn.ID
                                         WHERE inv.IsSynchronization = 0 AND inv.Employee1ID = {id}
				                         GROUP BY exn.Name) as t2 ON t2.Name = t1.Name
						 WHERE (t1.LeftQuantity - t2.ReturnQuantity) > 0
                         ORDER BY Quantity DESC";

            return await _conn.QueryToListAsync<ScannedDataModel>(sql)
                .ConfigureAwait(false);
        }

    }
}
