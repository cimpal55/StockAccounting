using LinqToDB;
using LinqToDB.Data;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace StockAccounting.Core.Data.Repositories
{
    public class ScannedDataRepository : IScannedDataRepository
    {
        private readonly AppDataConnection _conn;
        public ScannedDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }
        public async Task UpdateScannedDataAsync(ScannedDataBaseModel item) =>
            await _conn
                .ScannedData
                .Where(x => x.Id == item.Id)
                .Set(x => x.ExternalDataId, item.ExternalDataId)
                .Set(x => x.Quantity, item.Quantity)
                .UpdateAsync()
                .ConfigureAwait(false);
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
            foreach (var item in barcodes)
            {
                var dbItem = await _conn.ScannedData
                                .Join(_conn.ExternalData, id => id.ExternalDataId, w => w.Id, (id, w) => new { id, w })
                                .Join(_conn.InventoryData, iv => iv.id.InventoryDataId, id => id.Id, (iv, id) => new { iv, id })
                                .Join(_conn.Employees, em2 => em2.id.Employee2Id, em => em.Id, (em2, em) => new { em2, em })
                                .FirstOrDefaultAsync(x => x.em2.iv.w.Barcode == item.Barcode &&
                                                          string.Join(" ", x.em.Name, x.em.Surname, x.em.Code).Contains(item.Employee));

                if (dbItem != null)
                {
                    dbItem.em2.iv.id.Quantity--;

                    await _conn.InventoryData
                        .Where(x => x.Id == dbItem.em2.id.Id)
                        .Set(x => x.Updated, DateTime.Now)
                        .UpdateAsync()
                        .ConfigureAwait(false);

                    await _conn.ScannedData
                        .Where(x => x.Id == dbItem.em2.iv.id.Id)
                        .Set(x => x.Quantity, dbItem.em2.iv.id.Quantity)
                        .UpdateAsync()
                        .ConfigureAwait(false);
                } else {
                    await CreateDocumentForSynchronization(item)
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task CreateDocumentForSynchronization(SynchronizationModel item)
        {
            if (item != null && item.Barcode.Any() && item.Employee.Any())
            {
                try
                {
                    await _conn.BeginTransactionAsync();

                    var preparedData = await GetPreparedModelsForDocument(item)
                        .ConfigureAwait(false);

                    int docId = await _conn.InventoryData
                        .InsertWithInt32IdentityAsync(() => new InventoryDataModel
                        {
                            Employee1Id = preparedData[0],
                            Employee2Id = preparedData[1],
                            ManuallyAdded = false,
                            IsSynchronization = true,
                            Created = DateTime.Now
                        });

                    var datasca = new ScannedDataBaseModel()
                    {
                        InventoryDataId = docId,
                        ExternalDataId = preparedData[2],
                        Quantity = -1,
                        Created = DateTime.Now,
                    };

                    await _conn.InsertAsync(datasca);

                    await _conn.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await _conn.RollbackTransactionAsync();
                    throw;
                }
            }
        }

        public async Task<List<int>> GetPreparedModelsForDocument(SynchronizationModel item)
        {
            var employee1Id = await _conn
                .Employees
                .Where(x => x.IsManager == true)
                .Select(x => x.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            var employee2Id = await _conn
                .Employees
                .Where(x => string.Join(" ", x.Name, x.Surname, x.Code).Contains(item.Employee))
                .Select(x => x.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            var externalDataId = await _conn
                .ExternalData
                .Where(x => x.Barcode == item.Barcode)
                .Select(x => x.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            
            if(externalDataId == 0) {
                externalDataId = await _conn.ExternalData
                    .InsertWithInt32IdentityAsync(() => new ExternalDataModel
                    {
                        Barcode = item.Barcode,
                        ItemNumber = "- None -",
                        Name = "- None -",
                        PluCode = "- None -",
                        Unit = "- None -",
                        Updated = DateTime.Now,
                        Created = DateTime.Now
                    });
            }

            List<int> result = new List<int>
            {
                employee1Id, employee2Id, externalDataId
            };

            return result;
        }

        public async Task<IEnumerable<ScannedDataModel>> GetScannedDataByIdAsync(int inventoryDataId)
        {
            var query = from sd in _conn.ScannedData
                        join ex in _conn.ExternalData on sd.ExternalDataId equals ex.Id
                        where sd.InventoryDataId == inventoryDataId
                        select new ScannedDataModel
                        {
                            Id = sd.Id,
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
                    return @$"SELECT em.ID, (em.Name + ' ' + em.Surname + ' ' + em.Code) as FullName,
                                     ex.Name as Name, ex.ItemNumber,
                                     ex.PluCode, SUM(sc.Quantity) as Quantity, sc.Created
                                       FROM TBL_InventoryData iv
                                       JOIN TBL_ScannedData sc ON iv.ID = sc.InventoryDataID
                                       JOIN TBL_ExternalData ex on sc.ExternalDataId = ex.ID
            				  	       JOIN TBL_CONF_Employees em on em.ID = iv.Employee2ID
                                       WHERE iv.IsSynchronization = 1 AND iv.Employee2ID IN (employeesToReplace)
             	                       GROUP BY em.ID, ex.Name, ex.ItemNumber, ex.PluCode, sc.Created,
                                                (em.Name + ' ' + em.Surname + ' ' + em.Code)
									   ORDER BY Quantity DESC";

                case FileExport.Returned:
                    return @"SELECT em.ID, (em.Name + ' ' + em.Surname + ' ' + em.Code) as FullName,
                                    exn.Name as Name, exn.ItemNumber, exn.PluCode,
                                    scn.Quantity, scn.Created
			                            FROM TBL_ScannedData scn
			                            JOIN TBL_InventoryData inv on inv.ID = scn.InventoryDataID
			                            JOIN TBL_ExternalData exn on scn.ExternalDataId = exn.ID
			                            JOIN TBL_CONF_Employees em on em.ID = inv.Employee1ID
			                            WHERE inv.IsSynchronization = 0 AND inv.Employee1ID IN (employeesToReplace)
			                            GROUP BY exn.Name, em.ID, scn.Created, exn.ItemNumber, exn.PluCode, 
					                            scn.Quantity, (em.Name + ' ' + em.Surname + ' ' + em.Code)
			                            ORDER BY Quantity DESC";

                default:
                    return @$"SELECT t1.ID, t1.FullName, t1.Name, t1.ItemNumber,
                                   t1.PluCode, CASE WHEN t2.ReturnQuantity > 0 THEN (t1.LeftQuantity - t2.ReturnQuantity)
                                                                               ELSE t1.LeftQuantity END AS Quantity,
                                   t1.Created
                               FROM
                               (SELECT em.ID, (em.Name + ' ' + em.Surname + ' ' + em.Code) as FullName,
                                       ex.Name as Name, ex.ItemNumber,
                                       ex.PluCode, sc.Quantity as LeftQuantity, sc.Created
                                         FROM TBL_InventoryData iv
                                         JOIN TBL_ScannedData sc ON iv.ID = sc.InventoryDataID
                                         JOIN TBL_ExternalData ex on sc.ExternalDataId = ex.ID
            	                         JOIN TBL_CONF_Employees em on em.ID = iv.Employee2ID
                                         WHERE iv.IsSynchronization = 1 AND iv.Employee2ID IN (employeesToReplace)
             	                         GROUP BY em.ID, ex.Name, ex.ItemNumber, ex.PluCode, sc.Created,
												  sc.Quantity, (em.Name + ' ' + em.Surname + ' ' + em.Code)) as t1
                               LEFT JOIN
                               (SELECT exn.Name as Name, scn.Quantity as ReturnQuantity
                                      FROM TBL_ScannedData scn
                                      JOIN TBL_InventoryData inv on inv.ID = scn.InventoryDataID
                                      JOIN TBL_ExternalData exn on scn.ExternalDataId = exn.ID
                                      WHERE inv.IsSynchronization = 0 AND inv.Employee1ID IN (employeesToReplace)
                                      GROUP BY exn.Name, scn.Quantity) as t2 ON t2.Name = t1.Name
                                      ORDER BY LeftQuantity DESC";
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
            var query = from sd in _conn.ScannedData
                        join iv in _conn.InventoryData on sd.InventoryDataId equals iv.Id
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
    }
}
