using FirebirdSql.Data.FirebirdClient;
using LinqToDB;
using LinqToDB.AspNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Globalization;
using System.Data;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Core.Data.Services.Interfaces;
using StockAccounting.UsedStocksSynchronization.Utils.ServiceRegistration;

IConfiguration _configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  .Build();

var logFilePath = $"Logs/log-.txt";

#if (RELEASE)
    logFilePath = $"C:\\www\\StockAccounting\\UsedStocksSynchronization\\Logs\\log-.txt";
#endif

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Hour)
    .CreateLogger();

var serviceProvider = CreateServices();
var _externalDataRepository = serviceProvider.GetRequiredService<IExternalDataRepository>();
var _employeeRepository = serviceProvider.GetRequiredService<IEmployeeDataRepository>();
var _smtpEmailService = serviceProvider.GetRequiredService<ISmtpEmailService>();

using var scope = serviceProvider.CreateScope();
await SynchronizationAsync(scope.ServiceProvider);

IServiceProvider CreateServices()
{
    return new ServiceCollection()
        .AddLinqToDBContext<AppDataConnection>((provider, options) =>
        {
            var connectionString = _configuration["ConnectionStrings:Default"];

            return options
             .UseSqlServer(connectionString);
        })
        .AddUsedStocksSynchronizationRepositories(_configuration)
        .BuildServiceProvider();
}

async Task SynchronizationAsync(IServiceProvider serviceProvider)
{
    Log.Information("Synchronization start: {date}", DateTime.Now);

    Log.Information("---- Getting service trader connection string ----");
    string ConnectionString = _configuration["ConnectionStrings:Firebird"];
    Log.Debug("Connection string is: {ConnectionString}", ConnectionString);
    Log.Debug("");

    using (var connection = new FbConnection(ConnectionString))
    {
        connection.Open();

        var synchronizationDate = ReturnArgumentsDate();

        var stocks = await GetUsedScannedDataFromFirebirdAsync(connection, synchronizationDate);

        _smtpEmailService.SendEmail(stocks, synchronizationDate);

        foreach (var stock in stocks)
        {
            var employee = stock.Select(x => x.Employee).First();
            Log.Information("Used stocks sent to: {0}", employee);
        }
    }

    Log.Information("----Synchronization finished!----");
}

    async Task<IEnumerable<IEnumerable<SynchronizationModel>>> GetUsedScannedDataFromFirebirdAsync(FbConnection conn, DateTime date)
{
    List<SynchronizationModel> fromScanned = new();

    string[] arguments = Environment.GetCommandLineArgs();

    string synchronizationDate = date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

    Log.Information("Starting synchronization after date {0}", synchronizationDate);

    var usedList = new FbCommand($@"SELECT wr.WORKER_CODE as WORKER, sd.L1, sdc.DOC_SER_NR, sdc.DOC_NR, gd.BARCODE,
	                                       gd.CODE1, gd.CODE10, sd.AMOUNT, sd.ENTERED  
                                    FROM GOODS gd 
                                    JOIN SELL_DET sd ON gd.GOOD_ID = sd.GOOD_ID 
                                    JOIN SELL_DOC sdc ON sd.SELL_DOC_ID = sdc.SELL_DOC_ID
                                    JOIN SELL_DET_WORKERS sdw ON sdw.SELL_DET_ID = sd.SELL_DET_ID  
                                    JOIN WORKERS wr ON wr.WORKER_ID = sdw.WORKERS_ID
                                    WHERE CAST(sd.ENTERED AS DATE) = '{synchronizationDate}'
                                    AND gd.GOOD_TYPE_ID = 3", conn);

    var usedReader = usedList.ExecuteReader();
    while (usedReader.Read())
    {
        var externalData = new ExternalDataModel
        {
            Barcode = usedReader.GetString(4),
            ItemNumber = usedReader.GetString(5),
            Name = usedReader.GetString(1),
            PluCode = usedReader.GetString(6) == null ? "-" : usedReader.GetString(6),
            Quantity = usedReader.GetDecimal(7),
            Document = usedReader.GetString(2) + usedReader.GetString(3),
            Created = usedReader.GetDateTime(8)
        };

        var employee = usedReader.GetString(0);

        var used = new SynchronizationModel
        {
            Employee = employee,
            ExternalData = externalData,
            EmployeeEmail = _employeeRepository.GetEmployeeEmailByCode(employee)
        };

        if (used.EmployeeEmail == null)
            continue;

        fromScanned.Add(used);
    }

    var groupedUsedList = fromScanned
        .GroupBy(x => x.Employee)
        .Select(grp => grp.ToList())
        .ToList();

    Log.Information("Were found {0} employees to send used stocks", groupedUsedList.Count);

    return groupedUsedList;
}

DateTime ReturnArgumentsDate()
{
    string[] arguments = Environment.GetCommandLineArgs();

    int length = 1;

    if (arguments.Length > length)
    {
        DateTime.TryParse(arguments[length], out var date);
        return date;
    }

    return DateTime.Today.AddDays(-1); 
}