using LinqToDB;
using LinqToDB.Data;
using Serilog;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using System.Security.Cryptography.X509Certificates;
using static LinqToDB.Common.Configuration;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Repositories
{
    public class ScannedDataRepository : IScannedDataRepository
    {
        private readonly AppDataConnection _conn;
        public ScannedDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public async Task<ScannedDataBaseModel> ReturnScannedDataById(int scannedDataId) =>
            await _conn
                .ScannedData
                .Where(x => x.Id == scannedDataId)
                .FirstOrDefaultAsync();

        public async Task UpdateScannedDataAsync(ScannedDataBaseModel item) =>
            await _conn
                .ScannedData
                .Where(x => x.Id == item.Id)
                .Set(x => x.ExternalDataId, item.ExternalDataId)
                .Set(x => x.Quantity, item.Quantity)
                .UpdateAsync();

        public async Task<IEnumerable<SynchronizationModel>> GetBarcodesAsync()
        {
            var query = from sd in _conn.ScannedData
                        join ex in _conn.ExternalData on sd.ExternalDataId equals ex.Id
                        join iv in _conn.InventoryData on sd.InventoryDataId equals iv.Id
                        join em1 in _conn.Employees on iv.Employee1Id equals em1.Id
                        join em2 in _conn.Employees on iv.Employee2Id equals em2.Id
                        where iv.IsSynchronization == true
                        select new SynchronizationModel
                        {
                            Barcode = ex.Barcode, 
                            Employee = string.Join(" ", em2.Name, em2.Surname),
                        };

            return await query.ToListAsync();
        }

        public async Task SynchronizationWithServiceTrader(IEnumerable<SynchronizationModel> barcodes)
        {
            try
            {
                List<int> stocksSynchronization = new();
                StockDataBaseModel? stockData = new();
                foreach (var item in barcodes)
                {
                    var externalData = await _conn
                        .ExternalData
                        .FirstOrDefaultAsync(x => x.Barcode == item.Barcode);

                    if (externalData == null)
                    {
                        Log.Debug("External data with barcode {0} doesn't exist.", item.Barcode);
                        continue;
                    }

                    var employee = await _conn
                        .Employees
                        .FirstOrDefaultAsync(x => x.Code == item.Employee);

                    stockData = await _conn
                        .StockData
                        .FirstOrDefaultAsync(x => x.ExternalDataId == externalData.Id);

                    if (stockData != null && stockData.LastSynchronization < item.Created)
                    {
                        if (!stocksSynchronization.Contains(stockData.Id))
                            stocksSynchronization.Add(stockData.Id);

                        StockEmployeesBaseModel stockEmployee = new StockEmployeesBaseModel
                        {
                            DocumentSerialNumber = item.DocumentSerialNumber + item.DocumentNumber,
                            EmployeeId = employee.Id,
                            StockDataId = stockData.Id,
                            StockTypeId = (int)StockTypes.Used,
                            Quantity = item.Quantity * -1,
                            LastSynchronization = DateTime.Now,
                            Created = item.Created
                        };

                        await _conn.InsertAsync(stockEmployee);
                        Log.Debug("Stock employees model was inserted: {0}", stockEmployee);
                        Log.Debug("External data with barcode {0} successfully synchronized", item.Barcode);

                        stockData.Quantity = stockData.Quantity - item.Quantity;
                        await _conn.UpdateAsync(stockData);
                        continue;
                    }
                    else if (stockData == null)
                    {
                        stockData = new StockDataBaseModel
                        {
                            ExternalDataId = externalData.Id,
                            Quantity = 0 - item.Quantity,
                            LastSynchronization = DateTime.Today,
                            Created = DateTime.Now,
                        };

                        var stockDataId = await _conn.InsertWithInt32IdentityAsync(stockData);

                        StockEmployeesBaseModel stockEmployee = new StockEmployeesBaseModel
                        {
                            DocumentSerialNumber = item.DocumentSerialNumber + item.DocumentNumber,
                            EmployeeId = employee.Id,
                            StockDataId = stockDataId,
                            StockTypeId = (int)StockTypes.Used,
                            Quantity = item.Quantity * -1,
                            LastSynchronization = DateTime.Now,
                            Created = item.Created
                        };

                        await _conn.InsertAsync(stockEmployee);

                        if(!stocksSynchronization.Contains(stockDataId))
                            stocksSynchronization.Add(stockDataId);

                        Log.Debug("External data with barcode {0} successfully synchronized", item.Barcode);
                        continue;
                    }
                    else
                    {
                        Log.Debug("Last {0} barcode synchronization was made {1} but external data created {2}", item.Barcode, stockData.LastSynchronization, item.Created);
                        continue;
                    }
                }

                Log.Debug("Were synchronized {0} stocks", stocksSynchronization.Count);

                foreach (var id in stocksSynchronization)
                {

                    var stock = await _conn
                        .StockData
                        .Where(x => x.Id == id)
                        .FirstOrDefaultAsync();

                    stock.LastSynchronization = DateTime.Now;
                    
                    await _conn
                        .UpdateAsync(stock);

                    Log.Debug("Updated stock with id '{0}' last synchronization date", stock.Id);
                }

                await _conn.CommitTransactionAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<IEnumerable<ScannedDataModel>> GetScannedDataByDocumentIdAsync(int inventoryDataId)
        {
            var query = from sd in _conn.ScannedData
                        join ex in _conn.ExternalData on sd.ExternalDataId equals ex.Id
                        where sd.InventoryDataId == inventoryDataId
                        select new ScannedDataModel
                        {
                            Id = sd.Id,
                            DocumentNumber = sd.DocumentNumber,
                            DocumentSerialNumber = sd.DocumentSerialNumber,
                            ExternalDataId = sd.ExternalDataId,
                            Name = ex.Name,
                            ItemNumber = ex.ItemNumber,
                            PluCode = ex.PluCode,
                            InventoryDataId = sd.Id,
                            Quantity = sd.Quantity,
                        };

            return await query.ToListAsync();
        }

        public string FileExportMode(FileExport mode)
        {
            switch (mode)
            {
                case FileExport.Taken:
                    return @$"SELECT em.ID, (em2.Name + ' ' + em2.Surname + ' ' + em2.Code) as Manager,
	                                 (em.Name + ' ' + em.Surname + ' ' + em.Code) as Employee,
                                     ex.Name as Name, ex.Barcode, ex.ItemNumber,
                                     ex.PluCode, SUM(sc.Quantity) as Quantity, sc.Created
                                       FROM TBL_InventoryData iv
                                       JOIN TBL_ScannedData sc ON iv.ID = sc.InventoryDataID
                                       JOIN TBL_ExternalData ex on sc.ExternalDataId = ex.ID
            				  	       JOIN TBL_CONF_Employees em on em.ID = iv.Employee2ID
									   JOIN TBL_CONF_Employees em2 on em2.ID = iv.Employee1ID
                                       WHERE iv.IsSynchronization = 1 AND iv.Employee2ID IN (employeesToReplace)
             	                       GROUP BY em.ID, ex.Name, ex.ItemNumber, ex.PluCode, sc.Created, ex.Barcode,
                                                (em.Name + ' ' + em.Surname + ' ' + em.Code),
												(em2.Name + ' ' + em2.Surname + ' ' + em2.Code)
									   ORDER BY Created DESC";

                case FileExport.Returned:
                    return @"SELECT em.ID, (em2.Name + ' ' + em2.Surname + ' ' + em2.Code) as Manager, 
                                    (em.Name + ' ' + em.Surname + ' ' + em.Code) as Employee,
                                    exn.Name as Name, exn.Barcode, exn.ItemNumber, exn.PluCode,
                                    scn.Quantity, scn.Created
			                            FROM TBL_ScannedData scn
			                            JOIN TBL_InventoryData inv on inv.ID = scn.InventoryDataID
			                            JOIN TBL_ExternalData exn on scn.ExternalDataId = exn.ID
			                            JOIN TBL_CONF_Employees em on em.ID = inv.Employee1ID
										JOIN TBL_CONF_Employees em2 ON em2.ID = inv.Employee2ID
			                            WHERE inv.IsSynchronization = 0 AND inv.Employee1ID IN (employeesToReplace)
			                            GROUP BY exn.Name, em.ID, scn.Created, exn.ItemNumber, exn.PluCode, exn.Barcode,
					                             scn.Quantity, (em.Name + ' ' + em.Surname + ' ' + em.Code),
                                                 (em2.Name + ' ' + em2.Surname + ' ' + em2.Code)
			                            ORDER BY Created DESC";

                default:
                    return @$"SELECT t1.ID, t1.Manager, t1.FullName, t1.Name, t1.Barcode, t1.ItemNumber,
                                   t1.PluCode, CASE WHEN t2.ReturnQuantity > 0 THEN (t1.LeftQuantity - t2.ReturnQuantity)
                                                                               ELSE t1.LeftQuantity END AS Quantity,
                                   t1.Created
                               FROM
                               (SELECT em.ID, (em2.Name + ' ' + em2.Surname + ' ' + em2.Code) as Manager,
									   (em.Name + ' ' + em.Surname + ' ' + em.Code) as Employee,
                                       ex.Name as Name, ex.ItemNumber, ex.Barcode,
                                       ex.PluCode, sc.Quantity as LeftQuantity, sc.Created
                                         FROM TBL_InventoryData iv
                                         JOIN TBL_ScannedData sc ON iv.ID = sc.InventoryDataID
                                         JOIN TBL_ExternalData ex ON sc.ExternalDataId = ex.ID
            	                         JOIN TBL_CONF_Employees em ON em.ID = iv.Employee2ID
										 JOIN TBL_CONF_Employees em2 ON em2.ID = iv.Employee1ID
                                         WHERE iv.IsSynchronization = 1 AND iv.Employee2ID IN (employeesToReplace)
             	                         GROUP BY em.ID, ex.Name, ex.ItemNumber, ex.PluCode, ex.Barcode, sc.Created,
												  sc.Quantity, (em.Name + ' ' + em.Surname + ' ' + em.Code),
												  (em2.Name + ' ' + em2.Surname + ' ' + em2.Code)) as t1
                               LEFT JOIN
                               (SELECT exn.Name as Name, scn.Quantity as ReturnQuantity
                                      FROM TBL_ScannedData scn
                                      JOIN TBL_InventoryData inv ON inv.ID = scn.InventoryDataID
                                      JOIN TBL_ExternalData exn ON scn.ExternalDataId = exn.ID
                                      WHERE inv.IsSynchronization = 0 AND inv.Employee1ID IN (employeesToReplace)
                                      GROUP BY exn.Name, scn.Quantity) as t2 ON t2.Name = t1.Name
                                      ORDER BY Created DESC";
            }
        }

        public async Task<IEnumerable<ScannedReportExcelModel>> GetScannedReportDataAsync(IEnumerable<int> employeeList, FileExport mode)
        {
            var employees = string.Join(",", employeeList);

            string sql = FileExportMode(mode).Replace("employeesToReplace", employees);

            return await _conn.QueryToListAsync<ScannedReportExcelModel>(sql)
                .ConfigureAwait(false);
        }

        public bool IsSynchronizationDocument(int inventoryDataId)
        {
            var query = from iv in _conn.InventoryData
                        where iv.Id == inventoryDataId
                        select iv.IsSynchronization;

            return query.FirstOrDefault();
        }

        public int ReturnDocumentEmployees(int inventoryDataId)
        {
            var synchronization = IsSynchronizationDocument(inventoryDataId);

            var query = from iv in _conn.InventoryData
                        where iv.Id == inventoryDataId
                        select (synchronization ? iv.Employee2Id : iv.Employee1Id);

            return query.FirstOrDefault();
        }

        public string ReturnEmployeeNotificationEmail(int employeeId)
        {
            var query = from e in _conn.Employees
                        where e.Id == employeeId
                        select e.Email;

            return query.FirstOrDefault();
        }

        public async Task<Dictionary<string, string>> GetDocumentNumber(int employeeId, int inventoryDataId)
        {

            string? employeeCode;
            int? docNr;
            int? checkIfExists;
            string? documentSerialNumber;
            Dictionary<string, string> dict = new Dictionary<string, string>();

            employeeCode = await _conn.Employees
                        .Where(x => x.Id == employeeId)
                        .Select(x => x.Code)
                        .FirstOrDefaultAsync();

            checkIfExists = await _conn.ScannedData.Where(x => x.InventoryDataId == inventoryDataId)
                                                   .Take(1)
                                                   .Select(x => x.DocumentNumber)
                                                   .FirstOrDefaultAsync();

            if (checkIfExists == null)
            {
                docNr = await _conn.ScannedData.Where(x => x.DocumentSerialNumber == employeeCode)
                                               .OrderByDescending(x => x.DocumentNumber)
                                               .Take(1)
                                               .Select(x => x.DocumentNumber)
                                               .SingleOrDefaultAsync() + 1;

                if (docNr == null)
                    docNr = 1;
            }
            else
                docNr = checkIfExists;

            documentSerialNumber = await _conn.Employees.Where(x => x.Id == employeeId)
                                                        .Select(x => x.Code)
                                                        .FirstOrDefaultAsync();


            dict.Add("DocNr", docNr.ToString());
            dict.Add("DocSerNr", documentSerialNumber);

            return dict;
        }

        public async Task<StockEmployeesBaseModel> GetStockEmployeeByScannedData(ScannedDataBaseModel model) =>
            await _conn
                .StockEmployees
                .Where(x => x.DocumentSerialNumber + x.DocumentNumber == model.DocumentSerialNumber + model.DocumentNumber)
                .Where(x => x.Created.AddMilliseconds(-x.Created.Millisecond) == model.Created.AddMilliseconds(-model.Created.Millisecond))
                .FirstOrDefaultAsync() ?? new StockEmployeesBaseModel();
    }
}
