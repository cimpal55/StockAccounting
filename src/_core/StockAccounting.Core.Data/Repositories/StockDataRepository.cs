using LinqToDB;
using LinqToDB.Data;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.Data.Excel;
using StockAccounting.Core.Data.Repositories.Interfaces;
using static LinqToDB.Common.Configuration;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Repositories
{
    public class StockDataRepository : IStockDataRepository
    {
        private readonly AppDataConnection _conn;
        public StockDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public async Task<IEnumerable<StockDataModel>> GetStockDataAsync()
        {
            var sql = @"SELECT st.ID, ex.Name, ex.Barcode, ex.Unit,
                               ex.PluCode, ex.ItemNumber, st.Quantity as LeftQuantity
                        FROM TBL_StockData st
                        JOIN TBL_ExternalData ex ON st.ExternalDataID = ex.ID
						JOIN TBL_Stock_Employees ste ON ste.StockDataID = st.ID
                        GROUP BY st.ID, ex.Name, ex.Barcode, ex.PluCode, ex.ItemNumber,
                                 ex.Unit, st.Quantity";

            return await _conn.QueryToListAsync<StockDataModel>(sql)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<StockEmployeesModel>> GetStockDetailsByIdAsync(int id)
        {
            var sql = @$"SELECT (em.Name + ' ' + em.Surname + ' ' + em.Code) as Employee,
                                st.DocumentSerialNumber, st.DocumentNumber,
	                            ABS(st.Quantity) as Quantity, stt.Name as Type,
								st.Created, ex.Name as StockName
		                FROM TBL_Stock_Employees st
		                JOIN TBL_CONF_Employees em ON st.EmployeeID = em.ID
						JOIN TBL_StockTypes stt ON st.StockTypeID = stt.ID
						JOIN TBL_StockData sd ON sd.ID = st.StockDataID
						JOIN TBL_ExternalData ex ON ex.ID = sd.ExternalDataID
                        WHERE st.StockDataID = {id}
		                GROUP BY em.ID, st.Quantity, stt.Name, st.Created, ex.Name,
                                 st.DocumentSerialNumber, st.DocumentNumber,
                                 (em.Name + ' ' + em.Surname + ' ' + em.Code)";

            return await _conn.QueryToListAsync<StockEmployeesModel>(sql)
                .ConfigureAwait(false);
        }
        public async Task<StockDataBaseModel> CheckIfStockExists(int externalDataId) =>
            await _conn
                .StockData
                .FirstOrDefaultAsync(x => x.ExternalDataId == externalDataId)
                .ConfigureAwait(false);

        public async Task<bool> CheckIfStockEmployeeExists() =>
            await _conn
                .StockEmployees
                .AnyAsync();

        public async Task<List<string>> ReturnStockEmployeesCodes()
        {
            var sql = $@"SELECT em.Code
	                        FROM TBL_Stock_Employees st
	                        JOIN TBL_CONF_Employees em ON st.EmployeeID = em.ID
                            GROUP BY em.Code";

            return await _conn.QueryToListAsync<string>(sql)
                .ConfigureAwait(false);
        }

        public async Task InsertStockData(StockEmployeesModel stockEmployeeData)
        {
            int stockId;
            var stock = await CheckIfStockExists(stockEmployeeData.ExternalDataId);

            if (!stockEmployeeData.IsSynchronization)
                stockEmployeeData.Quantity *= -1;

            if (stock == null)
            {
                stockId = await _conn.StockData
                    .InsertWithInt32IdentityAsync(() => new StockDataBaseModel
                    {
                        ExternalDataId = stockEmployeeData.ExternalDataId,
                        Quantity = stockEmployeeData.Quantity,
                        LastSynchronization = DateTime.Now,
                        Created = DateTime.Now,
                    });

                stockEmployeeData.StockDataId = stockId;
            }
            else 
            {
                stockEmployeeData.StockDataId = stock.Id;
                stock.Quantity += stockEmployeeData.Quantity;

                await _conn.UpdateAsync(stock);
            }

            await _conn.StockEmployees
                .Value(x => x.DocumentNumber, stockEmployeeData.DocumentNumber)
                .Value(x => x.DocumentSerialNumber, stockEmployeeData.DocumentSerialNumber)
                .Value(x => x.StockDataId, stockEmployeeData.StockDataId)
                .Value(x => x.EmployeeId, stockEmployeeData.EmployeeId)
                .Value(x => x.StockTypeId, stockEmployeeData.StockTypeId)
                .Value(x => x.Quantity, stockEmployeeData.Quantity)
                .Value(x => x.Created, stockEmployeeData.Created)
                .InsertAsync();
        }

        public string FileExportMode(FileExport mode)
        {
            switch (mode)
            {
                case FileExport.Taken:
                    return "AND st.StockTypeID = 1";

                case FileExport.Returned:
                    return "AND st.StockTypeID = 2";

                default:
                    return "";
            }
        }

        public async Task<IEnumerable<StockReportExcelModel>> GetStockReportDataAsync(IEnumerable<int> stocksList, FileExport mode)
        {
            var stocks = string.Join(",", stocksList);
            var condition = FileExportMode(mode);

            string sql = $@"SELECT CONCAT(st.DocumentSerialNumber, st.DocumentNumber) as DocumentNumber, 
                                   (em.Name + ' ' + em.Surname + ' ' + em.Code) as Employee, 
	                               ABS(st.Quantity) as Quantity, stt.Name as Type,
								   st.Created, ex.Name as StockName, ex.Barcode
		                    FROM TBL_Stock_Employees st
		                    JOIN TBL_CONF_Employees em ON st.EmployeeID = em.ID
						    JOIN TBL_StockTypes stt ON st.StockTypeID = stt.ID
						    JOIN TBL_StockData sd ON sd.ID = st.StockDataID
						    JOIN TBL_ExternalData ex ON ex.ID = sd.ExternalDataID
                            WHERE sd.ID IN ({stocks}) {condition}
		                    GROUP BY em.ID, st.Quantity, stt.Name, st.Created, ex.Name,
                                     (em.Name + ' ' + em.Surname + ' ' + em.Code),
                                     st.DocumentSerialNumber, st.DocumentNumber, ex.Barcode";

            return await _conn.QueryToListAsync<StockReportExcelModel>(sql)
                .ConfigureAwait(false);
        }

        public async Task UpdateStockQuantity(int stockDataId, decimal quantity)
        {
            var stock = await _conn
                .StockData
                .Where(x => x.Id == stockDataId)
                .FirstOrDefaultAsync();

            stock.Quantity += quantity;

            await _conn.UpdateAsync(stock);
        }

        public async Task<IEnumerable<StockEmployeesModel>> GetStockLeftQuantity(int stockDataId) {
            string sql = $@"SELECT (em.Name + ' ' + em.Surname + ' ' + em.Code) as Employee,
	                            SUM(st.Quantity) as Quantity, ex.Name as StockName
		                FROM TBL_Stock_Employees st
		                JOIN TBL_CONF_Employees em ON st.EmployeeID = em.ID
						JOIN TBL_StockData sd ON sd.ID = st.StockDataID
						JOIN TBL_ExternalData ex ON ex.ID = sd.ExternalDataID
						WHERE st.StockDataID = {stockDataId}
		                GROUP BY st.StockDataID, ex.Name,
                                 (em.Name + ' ' + em.Surname + ' ' + em.Code)";
            
            return await _conn.QueryToListAsync<StockEmployeesModel>(sql)
                .ConfigureAwait(false);
        }
    }
}
