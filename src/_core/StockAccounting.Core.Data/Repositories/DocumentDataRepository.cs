using LinqToDB;
using LinqToDB.Data;
using Serilog;
using static LinqToDB.Common.Configuration;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.Data.StockEmployees;
using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.Core.Data.Repositories
{
    public class DocumentDataRepository : IDocumentDataRepository
    {
        private readonly AppDataConnection _conn;
        public DocumentDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public IQueryable<DocumentDataModel> GetDocumentDataQueryable()
        {
            var query = from inv in _conn.DocumentData
                join emp in _conn.Employees on inv.Employee1Id equals emp.Id
                join emp2 in _conn.Employees on inv.Employee2Id equals emp2.Id
                select new DocumentDataModel
                {
                    Id = inv.Id,
                    Employee1Id = emp.Id,
                    Employee2Id = emp2.Id,
                    Employee1 = emp.Name + " " + emp.Surname + " " + emp.Code,
                    Employee2 = emp2.Name + " " + emp2.Surname + " " + emp2.Code,
                    DocumentType = inv.DocumentType,
                    Created = inv.Created.AddSeconds(-inv.Created.Second),
                    IsSynchronization = inv.IsSynchronization,
                    Updated = inv.Updated,
                    ManuallyAdded = inv.ManuallyAdded,
                };

            return query;
        }

        public async Task<DocumentDataModel> GetDocumentDataByIdAsync(int id)
        {
            var query = from inv in _conn.DocumentData
                join emp in _conn.Employees on inv.Employee1Id equals emp.Id
                join emp2 in _conn.Employees on inv.Employee2Id equals emp2.Id
                where inv.Id == id
                select new DocumentDataModel
                {
                    Id = inv.Id,
                    Employee1Id = emp.Id,
                    Employee2Id = emp2.Id,
                    Employee1 = string.Join(" ", emp.Name, emp.Surname, emp.Code),
                    Employee2 = string.Join(" ", emp2.Name, emp2.Surname, emp2.Code),
                    DocumentType = inv.DocumentType,
                    Created = inv.Created.AddSeconds(-inv.Created.Second),
                };
            return await query.FirstOrDefaultAsync();
        }

        public IQueryable<DocumentDataModel> GetDocumentDataSearchTextQueryable(string searchText)
        {
            var query = from inv in _conn.DocumentData
                join emp in _conn.Employees on inv.Employee1Id equals emp.Id
                join emp2 in _conn.Employees on inv.Employee2Id equals emp2.Id
                where (emp.Name + emp.Surname + emp.Code).Contains(searchText)
                      || (emp2.Name + emp2.Surname + emp2.Code).Contains(searchText)
                      || inv.Created.ToString().Contains(searchText)
                orderby inv.Created descending
                select new DocumentDataModel
                {
                    Id = inv.Id,
                    Employee1Id = emp.Id,
                    Employee2Id = emp2.Id,
                    Employee1 = string.Join(" ", emp.Name, emp.Surname, emp.Code),
                    Employee2 = string.Join(" ", emp2.Name, emp2.Surname, emp2.Code),
                    Created = inv.Created,
                    DocumentType = inv.DocumentType,
                    IsSynchronization = inv.IsSynchronization,
                    ManuallyAdded = inv.ManuallyAdded,
                    Updated = inv.Updated
                };

            return query;
        }


        public async Task UpdateDocumentDataAsync(DocumentDataBaseModel item) =>
            await _conn
                .DocumentData
                .Where(x => x.Id == item.Id)
                .Set(x => x.Employee1Id, item.Employee1Id)
                .Set(x => x.Employee2Id, item.Employee2Id)
                .Set(x => x.Created, item.Created)
                .Set(x => x.Updated, DateTime.Now)
                .UpdateAsync()
                .ConfigureAwait(false);

        public bool CheckIfDocumentHasScannedData(int documentId) =>
            _conn
                .ScannedData
                .Where(x => x.DocumentDataId == documentId)
                .Any();

        public async Task<List<ScannedDataBaseModel>> ReturnDocumentScannedData(int documentId) =>
            await _conn
                .ScannedData
                .Where(x => x.DocumentDataId == documentId)
                .ToListAsync();

        public async Task<List<StockEmployeesBaseModel>> ReturnStockEmployeesByDocumentNumber(ScannedDataBaseModel data) =>
            await _conn
                .StockEmployees
                .Where(x => x.DocumentSerialNumber == data.DocumentSerialNumber)
                .Where(x => x.DocumentNumber == data.DocumentNumber)
                .ToListAsync();


        public async Task<List<StockEmployeesBaseModel>> ReturnStockEmployeesBySerialNumber(string serialNumber) =>
            await _conn
                .StockEmployees
                .Where(x => x.DocumentSerialNumber == serialNumber)
                .ToListAsync();

        public async Task<bool> CheckDocumentSynchronizationAsync(int employeeId)
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

        public async Task<bool> ReturnDocumentSynchronizationAsync(int documentId) =>
            await _conn
                .DocumentData
                .Where(x => x.Id == documentId)
                .Select(x => x.IsSynchronization)
                .FirstOrDefaultAsync();

        public async Task<int> ReturnDocumentIdIfExists(DocumentDataBaseModel item) =>
            await _conn
                .DocumentData
                .Where(x => x.Employee1Id == item.Employee1Id
                    && x.Employee2Id == item.Employee2Id
                    && x.Created.Date == item.Created.Date
                    && x.Created.Hour == item.Created.Hour
                    && x.Created.Minute == item.Created.Minute
                    && x.Created.Second == item.Created.Second)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

        public async Task<int> InsertWithIdentityAsync(DocumentDataBaseModel item)
        {
            var documentData = await _conn
                .DocumentData
                .Where(x => x.Employee1Id == item.Employee1Id
                    && x.Employee2Id == item.Employee2Id
                    && x.Created.Date == item.Created.Date
                    && x.Created.Hour == item.Created.Hour
                    && x.Created.Minute == item.Created.Minute
                    && x.Created.Second == item.Created.Second)
                .FirstOrDefaultAsync();

            if (documentData == null)
            {
                return await _conn
                    .InsertWithInt32IdentityAsync(item);
            }

            return documentData.Id;
        }
    }
}
