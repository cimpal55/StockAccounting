using LinqToDB;
using LinqToDB.Tools;
using Serilog;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Enums;
using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.Data.StockEmployees;
using StockAccounting.Core.Data.Models.Data.ToolkitHistory;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Core.Data.Services.Interfaces;
using static LinqToDB.Common.Configuration;
using static StockAccounting.Core.Data.Resources.Columns;
using IDocumentDataRepository = StockAccounting.Api.Repositories.Interfaces.IDocumentDataRepository;
using IExternalDataRepository = StockAccounting.Api.Repositories.Interfaces.IExternalDataRepository;
using StockTypes = StockAccounting.Core.Data.Enums.StockTypes;

namespace StockAccounting.Api.Repositories
{
    public class DocumentDataRepository : IDocumentDataRepository
    {
        private readonly AppDataConnection _conn;
        private readonly IExternalDataRepository _externalDataRepository;
        private readonly IScannedInventoryDataRepository _scannedInventoryDataRepository;
        private readonly IGenericRepository<ToolkitHistoryModel> _toolkitHistoryRepository;
        private readonly IStockDataRepository _stockDataRepository;
        private readonly IConfiguration _configuration;
        private readonly ISmtpEmailService _smtpEmailService;
        public DocumentDataRepository(
            AppDataConnection conn,
            IExternalDataRepository externalDataRepository,
            IConfiguration configuration,
            ISmtpEmailService smtpEmailService,
            IStockDataRepository stockDataRepository,
            IGenericRepository<ToolkitHistoryModel> toolkitHistoryRepository, 
            IScannedInventoryDataRepository scannedInventoryDataRepository)
        {
            _conn = conn;
            _externalDataRepository = externalDataRepository;
            _configuration = configuration;
            _smtpEmailService = smtpEmailService;
            _stockDataRepository = stockDataRepository;
            _toolkitHistoryRepository = toolkitHistoryRepository;
            _scannedInventoryDataRepository = scannedInventoryDataRepository;
        }

        private static decimal GetAdjustmentValue(decimal target, decimal current) =>
            target - current;

        public async Task<List<DocumentDataModel>> GetDocumentData() =>
            await _conn
                .DocumentData
                .ToListAsync();

        public async Task<int> InsertDocumentWithIdentityAsync(DocumentDataModel document) =>
            await _conn
                .DocumentData
                .InsertWithInt32IdentityAsync(() => new DocumentDataModel
                {
                    Employee1Id = document.Employee1Id,
                    Employee2Id = document.Employee2Id,
                    IsSynchronization = document.IsSynchronization,
                    ManuallyAdded = document.ManuallyAdded,
                    DocumentType = document.DocumentType,
                    Created = document.Created,
                });

        public async Task InsertDetailsAfterInventory(IEnumerable<ScannedInventoryDataRecord> details, int docId)
        {
            await _conn.BeginTransactionAsync();

            try
            {
                var employeeId = await _conn.DocumentData
                    .Where(x => x.Id == docId)
                    .Select(x => x.Employee2Id)
                    .FirstOrDefaultAsync();

                var employeeCode = await _conn.Employees
                    .Where(x => x.Id == employeeId)
                    .Select(x => x.Code)
                    .FirstOrDefaultAsync();

                var documentSerialNumber = string.Format("{0}_{1}_{2:yyyy_MM_dd}", StockTypes.Inventory.ToString(),
                    employeeCode, DateTime.Now);

                foreach (var item in details)
                {
                    var externalData = await _externalDataRepository.GetExternalDataByBarcode(item.Barcode);
                    var currentQuantity = await _scannedInventoryDataRepository.GetDetailQuantityByEmployeeIdAsync(employeeId, externalData.Id);
                    var quantity = GetAdjustmentValue(item.CheckedQuantity, currentQuantity);

                    var datasca = new ScannedDataBaseModel()
                    {
                        DocumentDataId = docId,
                        DocumentSerialNumber = documentSerialNumber,
                        InventoryDataId = null,
                        ExternalDataId = externalData.Id,
                        Quantity = quantity,
                        Created = DateTime.Now,
                    };

                    await _conn.InsertAsync(datasca);

                    var stockEmployeeData = new StockEmployeesModel()
                    {
                        IsSynchronization = false,
                        DocumentSerialNumber = documentSerialNumber,
                        ExternalDataId = externalData.Id,
                        EmployeeId = employeeId,
                        StockTypeId = (int)StockTypes.Inventory,
                        Quantity = quantity,
                        Created = DateTime.Now,
                    };

                    await _stockDataRepository.InsertStockDataAfterInventory(stockEmployeeData);
                }

                await _conn.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _conn.RollbackTransactionAsync();
                Log.Information("API_InsertDocumentDataAsync {message}", ex.Message);
                throw;
            }
        }

        public async Task InsertData(ScannedModel data)
        {
            var warehouseEmployees = await _conn.Employees.Where(x => x.IsManager == true)
                                                          .Select(x => x.Id)
                                                          .ToListAsync();

            if (warehouseEmployees.Contains(data.documentData.Employee1Id) || warehouseEmployees.Contains(data.documentData.Employee2Id))
                await InsertDocumentDataAsync(data, warehouseEmployees);
            else
                await MoveStocksToOtherEmployeeAsync(data);
        }

        public async Task MoveStocksToOtherEmployeeAsync(ScannedModel data)
        {

            try
            {
                int docId;
                string emailTo = _conn.Employees
                                      .FirstOrDefault(x => x.Id == data.documentData.Employee2Id).Email;
                bool isSynchronization = true;
                List<ScannedDataBaseModel> scannedData = new();

                await _conn.BeginTransactionAsync();

                var firstEmployee = data.documentData.Employee1Id;
                var secondEmployee = data.documentData.Employee2Id;

                docId = await _conn.DocumentData
                    .InsertWithInt32IdentityAsync(() => new DocumentDataModel
                    {
                        Employee1Id = firstEmployee,
                        Employee2Id = secondEmployee,
                        IsSynchronization = isSynchronization,
                        DocumentType = (int)StockTypes.Moved,
                        Created = DateTime.Now
                    });

                var firstEmployeeCode = _conn.Employees
                                        .Where(x => x.Id == firstEmployee)
                                        .Select(x => x.Code)
                                        .FirstOrDefault();

                var secondEmployeeCode = _conn.Employees
                                         .Where(x => x.Id == secondEmployee)
                                         .Select(x => x.Code)
                                         .FirstOrDefault();

                string documentSerialNumber = firstEmployeeCode + "->" + secondEmployeeCode;

                int lastNr = _conn.ScannedData.Where(x => x.DocumentSerialNumber == documentSerialNumber)
                                              .OrderByDescending(x => x.DocumentNumber)
                                              .Take(1)
                                              .Select(x => x.DocumentNumber)
                                              .FirstOrDefault() + 1 ?? 1;


                foreach (var item in data.scannedData)
                {
                    var datasca = new ScannedDataBaseModel()
                    {
                        DocumentDataId = docId,
                        DocumentSerialNumber = documentSerialNumber,
                        DocumentNumber = lastNr,
                        ExternalDataId = item.Id,
                        Quantity = item.Quantity,
                        Created = DateTime.Now,
                    };

                    await _conn.InsertAsync(datasca);

                    if (isSynchronization)
                        scannedData.Add(datasca);

                    var movedFromEmployeeData = new StockEmployeesModel()
                    {
                        IsSynchronization = isSynchronization,
                        DocumentSerialNumber = documentSerialNumber,
                        DocumentNumber = lastNr,
                        ExternalDataId = item.Id,
                        EmployeeId = data.documentData.Employee1Id,
                        StockTypeId = (int)StockTypes.Moved,
                        Quantity = item.Quantity * -1,
                        Created = DateTime.Now,
                    };

                    await _stockDataRepository.InsertStockData(movedFromEmployeeData);

                    var movedToEmployeeData = new StockEmployeesModel()
                    {
                        IsSynchronization = isSynchronization,
                        DocumentSerialNumber = documentSerialNumber,
                        DocumentNumber = lastNr,
                        ExternalDataId = item.Id,
                        EmployeeId = data.documentData.Employee2Id,
                        StockTypeId = (int)StockTypes.Moved,
                        Quantity = item.Quantity,
                        Created = DateTime.Now,
                    };

                    await _stockDataRepository.InsertStockData(movedToEmployeeData);
                }

                await _conn.CommitTransactionAsync();

                if (isSynchronization)
                    _smtpEmailService.SendEmail(emailTo, scannedData);
            }
            catch (Exception ex)
            {
                await _conn.RollbackTransactionAsync();
                Log.Information("API_InsertDocumentDataAsync {message}", ex.Message);
                throw;
            }
        }

        public async Task<int?> InsertDocumentDataAsync(ScannedModel data, List<int>? warehouseEmployees)
        {
            int docId;
            string emailTo = string.Empty;
            bool isSynchronization = true;
            List<ScannedDataBaseModel> scannedData = new();

            if (warehouseEmployees.Contains(data.documentData.Employee2Id))
            {
                isSynchronization = false;
            }
            else
            {
                emailTo = _conn.Employees
                               .FirstOrDefault(x => x.Id == data.documentData.Employee2Id).Email;
            }

            try
            {
                await _conn.BeginTransactionAsync();

                docId = await _conn.DocumentData
                    .InsertWithInt32IdentityAsync(() => new DocumentDataModel
                    {
                        Employee1Id = data.documentData.Employee1Id,
                        Employee2Id = data.documentData.Employee2Id,
                        IsSynchronization = isSynchronization,
                        DocumentType = isSynchronization == true ? (int)StockTypes.Taken : (int)StockTypes.Returned,
                        Created = DateTime.Now
                    });

                var employee = isSynchronization == false ? data.documentData.Employee1Id
                                                          : data.documentData.Employee2Id;

                var employeeCode = _conn.Employees
                                        .Where(x => x.Id == employee)
                                        .Select(x => x.Code)
                                        .FirstOrDefault();

                var lastNr = _conn.ScannedData.Where(x => x.DocumentSerialNumber == employeeCode)
                                              .OrderByDescending(x => x.DocumentNumber)
                                              .Take(1)
                                              .Select(x => x.DocumentNumber)
                                              .FirstOrDefault() + 1 ?? 1;

                foreach (var item in data.scannedData)
                {
                    var datasca = new ScannedDataBaseModel()
                    {
                        DocumentDataId = docId,
                        DocumentSerialNumber = employeeCode,
                        DocumentNumber = lastNr,
                        ExternalDataId = item.Id,
                        Quantity = item.Quantity,
                        Created = DateTime.Now,
                    };

                    await _conn.InsertAsync(datasca);

                    if (isSynchronization)
                        scannedData.Add(datasca);

                    var stockEmployeeData = new StockEmployeesModel()
                    {
                        IsSynchronization = isSynchronization,
                        DocumentSerialNumber = employeeCode,
                        DocumentNumber = lastNr,
                        ExternalDataId = item.Id,
                        EmployeeId = isSynchronization ? data.documentData.Employee2Id : data.documentData.Employee1Id,
                        StockTypeId = isSynchronization ? (int)StockTypes.Taken : (int)StockTypes.Returned,
                        Quantity = item.Quantity,
                        Created = DateTime.Now,
                    };

                    await _stockDataRepository.InsertStockData(stockEmployeeData);
                }

                foreach (var item in data.usedToolkitData)
                {
                    var usedToolkit = new ToolkitHistoryModel()
                    {
                        EmployeeId = employee,
                        ToolkitId = item.ToolkitId,
                        Created = DateTime.Now,
                    };

                    await _toolkitHistoryRepository.InsertAsync(usedToolkit);
                }

                await _conn.CommitTransactionAsync();

                if (isSynchronization)
                    _smtpEmailService.SendEmail(emailTo, scannedData);

            }
            catch (Exception ex)
            {
                await _conn.RollbackTransactionAsync();
                Log.Information("API_InsertDocumentDataAsync {message}", ex.Message);
                throw;
            }

            return docId;
        }
    }
}
