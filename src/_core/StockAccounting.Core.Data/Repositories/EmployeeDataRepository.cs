using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Tools;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data.EmployeeData;
using StockAccounting.Core.Data.Models.Data.StockData;
using StockAccounting.Core.Data.Models.DataTransferObjects;
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

        public IQueryable<EmployeeDataModel> GetEmployeesQueryable()
        {
            return _conn.Employees;
        }

        public IQueryable<EmployeeDataModel> GetEmployeesSearchTextQueryable(string searchText)
        {
            var query = from e in _conn.Employees
                where Sql.Like(e.Name + " " + e.Surname + " " + e.Code, $"%{searchText}%")
                select e;

            return query;
        }

        public async Task<int> GetEmployeeIdByCode(string code) =>
            await _conn
                .Employees
                .Where(x => x.Code == code)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

        public string GetEmployeeEmailByCode(string code) =>
            _conn
                .Employees
                .Where(x => x.Code == code)
                .Select(x => x.Email)
                .FirstOrDefault();

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

        public IQueryable<EmployeeDetailsListModel> GetEmployeeDetailsByIdQueryable(int id)
        {
            var sql = @$"SELECT ex.Name, ex.Barcode, ex.ItemNumber, 
	                            ex.PluCode, SUM(ste.Quantity) as Quantity,
                                (em.Name + ' ' + em.Surname + ' ' + em.Code) as Employee,
                                em.Id as EmployeeId, ex.Id as ExternalDataId
	                         FROM TBL_Stock_Employees ste
	                         JOIN TBL_StockData st ON ste.StockDataID = st.ID
	                         JOIN TBL_ExternalData ex ON st.ExternalDataID = ex.ID
	                         JOIN TBL_CONF_Employees em ON ste.EmployeeID = em.ID
	                         WHERE em.ID = {id}
	                         GROUP BY ex.Name, ex.Barcode, ex.ItemNumber, ex.PluCode,
                                      (em.Name + ' ' + em.Surname + ' ' + em.Code), 
                                      em.Id, ex.Id
	                         HAVING SUM(ste.Quantity) != 0";

            return _conn.FromSql<EmployeeDetailsListModel>(sql);
        }

        public IQueryable<EmployeeDetailLeftQuantityListModel> GetEmployeeDetailLeftQuantityByIdQueryable(int employeeId, int externalDataId)
        {
            var sql = @$"SELECT ste.DocumentSerialNumber, ste.DocumentNumber,
                                ex.Name, ex.Barcode, ex.ItemNumber, 
	                            ex.PluCode, SUM(ste.Quantity) as Quantity,
                                (em.Name + ' ' + em.Surname + ' ' + em.Code) as Employee,
                                em.Id as EmployeeId, ex.ID as ExternalDataID,
								ss.Name as Type, ste.Created
	                         FROM TBL_Stock_Employees ste
	                         JOIN TBL_StockData st ON ste.StockDataID = st.ID
	                         JOIN TBL_ExternalData ex ON st.ExternalDataID = ex.ID
	                         JOIN TBL_CONF_Employees em ON ste.EmployeeID = em.ID
							 JOIN TBL_StockTypes ss ON ste.StockTypeID = ss.ID
	                         WHERE em.ID = {employeeId} and ex.ID = {externalDataId}
	                         GROUP BY ex.Name, ex.Barcode, ex.ItemNumber, ex.PluCode,
                                      (em.Name + ' ' + em.Surname + ' ' + em.Code), 
                                      em.Id, ex.ID, ste.Quantity, ss.Name, ste.Created,
                                      ste.DocumentSerialNumber, ste.DocumentNumber";

            return _conn.FromSql<EmployeeDetailLeftQuantityListModel>(sql);
        }
    }
}
