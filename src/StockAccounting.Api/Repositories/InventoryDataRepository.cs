using LinqToDB;
using Serilog;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Services.Interfaces;

namespace StockAccounting.Api.Repositories
{
    public class InventoryDataRepository : IInventoryDataRepository
    {
        private readonly AppDataConnection _conn;
        private readonly IExternalDataRepository _externalDataRepository;
        private readonly Core.Data.Repositories.Interfaces.IGenericRepository<ToolkitHistoryModel> _toolkitHistoryRepository;
        private readonly Core.Data.Repositories.Interfaces.IStockDataRepository _stockDataRepository;
        private readonly IConfiguration _configuration;
        private readonly ISmtpEmailService _smtpEmailService;
        public InventoryDataRepository(
            AppDataConnection conn,
            IExternalDataRepository externalDataRepository,
            IConfiguration configuration,
            ISmtpEmailService smtpEmailService,
            Core.Data.Repositories.Interfaces.IStockDataRepository stockDataRepository,
            Core.Data.Repositories.Interfaces.IGenericRepository<ToolkitHistoryModel> toolkitHistoryRepository)
        {
            _conn = conn;
            _externalDataRepository = externalDataRepository;
            _configuration = configuration;
            _smtpEmailService = smtpEmailService;
            _stockDataRepository = stockDataRepository;
            _toolkitHistoryRepository = toolkitHistoryRepository;
        }
        public async Task<List<InventoryDataModel>> GetInventoryData() =>
            await _conn
                .InventoryData
                .ToListAsync();
        public async Task<int?> InsertInventoryDataAsync(ScannedModel data)
        {
            int docId;
            string emailTo;
            bool isSynchronization = true;
            List<ScannedDataBaseModel> scannedData = new();

            var warehouseEmployees = await _conn.Employees.Where(x => x.IsManager == true)
                                                          .Select(x => x.Id)
                                                          .ToListAsync();

            if (warehouseEmployees.Contains(data.inventoryData.Employee2Id))
            {
                isSynchronization = false;
                emailTo = _conn.Employees
                               .FirstOrDefault(x => x.Id == data.inventoryData.Employee1Id).Email;
            }
            else
            {
                emailTo = _conn.Employees
                               .FirstOrDefault(x => x.Id == data.inventoryData.Employee2Id).Email;
            }

            try
            {
                await _conn.BeginTransactionAsync();

                docId = await _conn.InventoryData
                    .InsertWithInt32IdentityAsync(() => new InventoryDataModel
                    {
                        Employee1Id = data.inventoryData.Employee1Id,
                        Employee2Id = data.inventoryData.Employee2Id,
                        IsSynchronization = isSynchronization,
                        Created = DateTime.Now
                    });

                var employee = isSynchronization == false ? data.inventoryData.Employee1Id
                                                          : data.inventoryData.Employee2Id;

                var employeeCode = _conn.Employees
                                        .Where(x => x.Id == employee)
                                        .Select(x => x.Code)
                                        .FirstOrDefault();

                var lastNr = _conn.ScannedData.Where(x => x.DocumentSerialNumber == employeeCode)
                                              .OrderByDescending(x => x.DocumentNumber)
                                              .Take(1)
                                              .Select(x => x.DocumentNumber)
                                              .FirstOrDefault() + 1 ?? 1;

                var documentSerialNumber = _conn.Employees.Where(x => x.Id == employee)
                                                          .Select(x => x.Code)
                                                          .FirstOrDefault();

                foreach (var item in data.scannedData)
                {
                    var datasca = new ScannedDataBaseModel()
                    {
                        InventoryDataId = docId,
                        DocumentSerialNumber = documentSerialNumber,
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
                        DocumentSerialNumber = documentSerialNumber,
                        DocumentNumber = lastNr,
                        ExternalDataId = item.Id,
                        EmployeeId = isSynchronization ? data.inventoryData.Employee2Id : data.inventoryData.Employee1Id,
                        StockTypeId = isSynchronization ? (int)StockTypes.Accepted : (int)StockTypes.Returned,
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
                Log.Information("API_InsertInventoryDataAsync {message}", ex.Message);
                throw;
            }

            return docId;
        }
    }
}
