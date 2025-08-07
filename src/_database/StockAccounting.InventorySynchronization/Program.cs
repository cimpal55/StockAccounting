using LinqToDB;
using LinqToDB.AspNet;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Enums;
using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.Data.StockEmployees;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Core.Data.Utils.ServiceRegistration;


var logFilePath = $"Logs/log-.txt";

#if (RELEASE)
    logFilePath = $"C:\\www\\StockAccounting\\InventorySynchronization\\Logs\\log-.txt";
#endif

IConfiguration _configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File($"Logs/log-.txt", rollingInterval: RollingInterval.Hour)
    .CreateLogger();

var connectionString = _configuration["ConnectionStrings:Default"];
var dbConnectionString = _configuration["ConnectionStrings:DbConnectionString"];

var serviceProvider = CreateServices();

var _documentDataRepository = serviceProvider.GetRequiredService<IDocumentDataRepository>();
var _externalDataRepository = serviceProvider.GetRequiredService<IExternalDataRepository>();
var _employeeRepository = serviceProvider.GetRequiredService<IEmployeeDataRepository>();
var _stockDataRepository = serviceProvider.GetRequiredService<IStockDataRepository>();
var _gScannedDataRepository = serviceProvider.GetRequiredService<IGenericRepository<ScannedDataBaseModel>>();

using var scope = serviceProvider.CreateScope();

await SynchronizationAsync(scope.ServiceProvider);

IServiceProvider CreateServices()
{
    return new ServiceCollection()
        .AddLinqToDBContext<AppDataConnection>((provider, options) =>
        {
            return options
             .UseSqlServer(dbConnectionString);
        })
        .AddStockAccountingRepositories()
        .BuildServiceProvider();
}

async Task SynchronizationAsync(IServiceProvider serviceProvider)
{
    List<SynchronizationModel> documentList;
    var dateStart = DateTime.Now;
    Log.Information("----------------Synchronization start----------------");
    Log.Information("Time: {date}", dateStart);
    Log.Debug("Connection string is: {connectionString}", connectionString);
    Log.Debug("Database connection string is: {dbConnectionString}", dbConnectionString);

    try
    {
        Log.Debug("");

        await GetExternalDataAsync(connectionString);
        documentList = GetDocumentDataList(connectionString);

        if (documentList.Count() > 0)
        {
            Log.Debug("Document data count: {count}", documentList.Count());
            Log.Debug("");

            Log.Debug("------Starting synchronizing database with document data-------");

            await CreateDocumentsFromInventory(documentList);

            Log.Debug("");
            Log.Debug("------Synchronization finished!------");
            var dateEnd = DateTime.Now;
            var time = dateEnd - dateStart;
            Log.Information("Time: {date}, ", dateEnd);
            Log.Information("Time spent: {time}", time);
        }
        else
        {
            Log.Debug("Document data wasn't found!");
        }
    }
    catch (Exception x)
    {
        Console.WriteLine(x.Message);
    }
}


async Task GetExternalDataAsync(string? connectionString)
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        string sql = @"SELECT ex.Barcode, ex.PluCode, ex.ItemNumber, ex.DisplayName, ex.Unit, ex.Created, ex.Updated
  FROM TBL_ScannedData sc
  JOIN TBL_ExternalData ex on sc.Barcode = ex.Barcode";


        using (var command = new SqlCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                try
                {
                    while (reader.Read())
                    {
                        var externalData = new ExternalDataModel()
                        {
                            Barcode = reader.GetString(0),
                            PluCode = reader.GetString(1),
                            ItemNumber = reader.GetString(2),
                            Name = reader.GetString(3),
                            Unit = reader.GetString(4),
                            Created = reader.GetDateTime(5),
                            Updated = reader.GetDateTime(5)
                        };

                        var id = await _externalDataRepository.GetOrCreateExternalDataId(externalData);
                        Log.Debug(id.ToString());
                    }
                }
                catch (Exception e)
                {
                    Log.Debug(reader.GetString(0));
                    Log.Debug(e.Message);
                }
            }
        }
    }
}

List<SynchronizationModel> GetDocumentDataList(string? connectionString)
{
    List<SynchronizationModel> documentList = new();


    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        Log.Debug("------Getting document data-------");

        connection.Open();
        string sql = @"SELECT RIGHT(iv.Name, len(iv.Name) - CHARINDEX('_', iv.Name)) as DocumentSerialNumber,
                              sc.Barcode, sc.PluCode, ex.ItemNumber, 
							  ex.DisplayName as Name, sc.Unit, sc.FinalQuantity as Quantity,
							  sc.Created, iv.Created as DocumentCreated
                          FROM TBL_InventoryData iv
                          JOIN TBL_ScannedData sc ON sc.InventoryDataID = iv.ID
						  JOIN TBL_ExternalData ex ON ex.Barcode = sc.Barcode
                          WHERE iv.Name LIKE 'Maš_%' OR iv.Name LIKE 'Mas_%' AND iv.Status = 'Checked'";


        using (var command = new SqlCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                try
                {
                    while (reader.Read())
                    {
                        var externalData = new ExternalDataModel()
                        {
                            Barcode = reader.GetString(1),
                            PluCode = reader.GetString(2),
                            ItemNumber = reader.GetString(3),
                            Name = reader.GetString(4),
                            Unit = reader.GetString(5),
                            Created = DateTime.Now,
                            Updated = DateTime.Now
                        };

                        var model = new SynchronizationModel()
                        {
                            DocumentSerialNumber = reader.GetString(0),
                            Quantity = reader.GetDecimal(6),
                            Created = reader.GetDateTime(7),
                            DocumentCreated = reader.GetDateTime(8),
                            ExternalData = externalData,
                        };

                        Log.Debug("{model}", model);
                        documentList.Add(model);
                    }
                }
                catch (Exception e)
                {
                    Log.Debug(reader.GetString(0));
                    Log.Debug(e.Message);
                }
            }
        }

        return documentList;
    }
}
async Task CreateDocumentsFromInventory(IEnumerable<SynchronizationModel> inventoryList)
{
    try
    {
        List<SynchronizationModel> newInventoryList = new();
        bool isSynchronization = true;
        int managerId = await _employeeRepository.GetEmployeeIdByCode("KSA");
        int employeeId;
        int documentId;
        string documentName;

        foreach (var item in inventoryList)
        {
            employeeId = await _employeeRepository.GetEmployeeIdByCode(item.DocumentSerialNumber.Substring(0, 3));

            DocumentDataBaseModel document = new()
            {
                Employee1Id = managerId,
                Employee2Id = employeeId,
                IsSynchronization = isSynchronization,
                Created = item.DocumentCreated,
                Updated = item.DocumentCreated,
                ManuallyAdded = false,
            };

            var existId = await _documentDataRepository.ReturnDocumentIdIfExists(document);
            if (existId == 0)
                newInventoryList.Add(item);
        }

        Log.Debug("Synchronizing {count} records", newInventoryList.Count());

        foreach (var item in newInventoryList)
        {
            employeeId = await _employeeRepository.GetEmployeeIdByCode(item.DocumentSerialNumber.Substring(0, 3));

            DocumentDataBaseModel document = new()
            {
                Employee1Id = managerId,
                Employee2Id = employeeId,
                IsSynchronization = isSynchronization,
                Created = item.DocumentCreated,
                Updated = item.DocumentCreated,
                DocumentType = (int)StockTypes.Inventory,
                ManuallyAdded = false,
            };

            documentId = await _documentDataRepository.InsertWithIdentityAsync(document);
            documentName = $"Mašīna_{item.DocumentSerialNumber}";

            var stockEmployeeData = new StockEmployeesModel()
            {
                IsSynchronization = isSynchronization,
                DocumentSerialNumber = documentName,
                ExternalDataId = await _externalDataRepository.GetOrCreateExternalDataId(item.ExternalData),
                EmployeeId = employeeId,
                StockTypeId = (int)StockTypes.Taken,
                Quantity = item.Quantity,
                Created = item.Created,
            };

            var scannedData = new ScannedDataBaseModel()
            {
                DocumentSerialNumber = documentName,
                ExternalDataId = stockEmployeeData.ExternalDataId,
                DocumentDataId = documentId,
                Created = stockEmployeeData.Created,
                Quantity = stockEmployeeData.Quantity,
            };

            await _gScannedDataRepository.InsertAsync(scannedData);
            await _stockDataRepository.InsertStockData(stockEmployeeData);

            Log.Debug("Document {documentName} with id {documentId} successfully synchronized model: {stockEmployeeData}", documentName, documentId, stockEmployeeData);
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex.Message);
    }
}
