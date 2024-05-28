using LinqToDB;
using LinqToDB.Data;
using Serilog;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using static LinqToDB.Common.Configuration;

namespace StockAccounting.Core.Data.Repositories
{
    public class InventoryDataRepository : IInventoryDataRepository
    {
        private readonly AppDataConnection _conn;
        public InventoryDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public async Task<IEnumerable<InventoryDataModel>> GetInventoryDataAsync()
        {
            var query = from inv in _conn.InventoryData
                        join emp in _conn.Employees on inv.Employee1Id equals emp.Id
                        join emp2 in _conn.Employees on inv.Employee2Id equals emp2.Id
                        orderby inv.Created descending
                        select new InventoryDataModel
                        {
                            Id = inv.Id,
                            Employee1Id = emp.Id,
                            Employee2Id = emp2.Id,
                            Employee1 = string.Join(" ", emp.Name, emp.Surname, emp.Code),
                            Employee2 = string.Join(" ", emp2.Name, emp2.Surname, emp2.Code),
                            Created = inv.Created.AddSeconds(-inv.Created.Second),
                        };
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<InventoryDataModel>> GetInventoryDataSearchTextAsync(string searchText)
        {
            var query = from inv in _conn.InventoryData
                        join emp in _conn.Employees on inv.Employee1Id equals emp.Id
                        join emp2 in _conn.Employees on inv.Employee2Id equals emp2.Id
                        where (emp.Name + emp.Surname + emp.Code).Contains(searchText) 
                                || (emp2.Name + emp2.Surname + emp2.Code).Contains(searchText) 
                                || inv.Created.ToString().Contains(searchText)
                        orderby inv.Created descending
                        select new InventoryDataModel
                        {
                            Id = inv.Id,
                            Employee1Id = emp.Id,
                            Employee2Id = emp2.Id,
                            Employee1 = string.Join(" ", emp.Name, emp.Surname, emp.Code),
                            Employee2 = string.Join(" ", emp2.Name, emp2.Surname, emp2.Code),
                            Created = inv.Created,
                        };
            return await query.ToListAsync();
        }

        public async Task UpdateInventoryDataAsync(InventoryDataBaseModel item) =>
            await _conn
                .InventoryData
                .Where(x => x.Id == item.Id)
                .Set(x => x.Employee1Id, item.Employee1Id)
                .Set(x => x.Employee2Id, item.Employee2Id)
                .Set(x => x.Created, item.Created)
                .Set(x => x.Updated, DateTime.Now)
                .UpdateAsync()
                .ConfigureAwait(false);

        public bool CheckIfDocumentHasScannedData(int inventoryId) =>
            _conn
                .ScannedData
                .Where(x => x.InventoryDataId == inventoryId)
                .Any();

        public async Task<List<ScannedDataBaseModel>> ReturnDocumentScannedData(int inventoryId) =>
            await _conn
                .ScannedData
                .Where(x => x.InventoryDataId == inventoryId)
                .ToListAsync();

        public async Task<List<StockEmployeesBaseModel>> ReturnStockEmployeesByDocumentNumber(ScannedDataBaseModel data) =>
            await _conn
                .StockEmployees
                .Where(x => x.DocumentSerialNumber + x.DocumentNumber == data.DocumentSerialNumber + data.DocumentNumber)
                .ToListAsync();

        public async Task<bool> CheckInventorySynchronizationAsync(int employeeId)
        {
            bool isSynchronization = true;

            var warehouseEmployees = await _conn.Employees.Where(x => x.IsManager == true)
                                                          .Select(x => x.Id)
                                                          .ToListAsync()
                                                          .ConfigureAwait(false);

            if (warehouseEmployees.Contains(employeeId))
                isSynchronization = false;

            return isSynchronization;
        }

        public async Task<bool> ReturnInventorySynchronizationAsync(int documentId) =>
            await _conn
                .InventoryData
                .Where(x => x.Id == documentId)
                .Select(x => x.IsSynchronization)
                .FirstOrDefaultAsync();

        public async Task<int> ReturnDocumentIdIfExists(InventoryDataBaseModel item) =>
            await _conn
                .InventoryData
                .Where(x => x.Employee1Id == item.Employee1Id
                    && x.Employee2Id == item.Employee2Id
                    && x.Created.Date == item.Created.Date
                    && x.Created.Hour == item.Created.Hour
                    && x.Created.Minute == item.Created.Minute
                    && x.Created.Second == item.Created.Second)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

        public async Task<int> InsertWithIdentityAsync(InventoryDataBaseModel item)
        {
            var inventoryData = await _conn
                .InventoryData
                .Where(x => x.Employee1Id == item.Employee1Id
                    && x.Employee2Id == item.Employee2Id
                    && x.Created.Date == item.Created.Date
                    && x.Created.Hour == item.Created.Hour
                    && x.Created.Minute == item.Created.Minute
                    && x.Created.Second == item.Created.Second)
                .FirstOrDefaultAsync();

            if (inventoryData == null)
            {
                return await _conn
                    .InsertWithInt32IdentityAsync(item);
            }

            return inventoryData.Id;
        }
    }
}
