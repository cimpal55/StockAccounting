using LinqToDB;
using LinqToDB.AspNet;
using Microsoft.Win32.TaskScheduler;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Utils.ServiceRegistration;
using StockAccounting.Core.Data.Repositories.Interfaces;
using System.Globalization;
using Serilog;
using Dayton.NetsuiteOAuth1RestApi.Services;
using Azure.Identity;

IConfiguration _configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  .AddEnvironmentVariables()
  .AddCommandLine(args)
  .Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File($"Logs/log.txt")
    .CreateLogger();

var serviceProvider = CreateServices();
var _repository = serviceProvider.GetRequiredService<IGenericRepository<EmployeeDataModel>>();
var _externalDataRepository = serviceProvider.GetRequiredService<IGenericRepository<ExternalDataModel>>();
var _employeeRepository = serviceProvider.GetRequiredService<IEmployeeDataRepository>();
var _scannedDataRepository = serviceProvider.GetRequiredService<IScannedDataRepository>();

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
        .AddStockAccountingRepositories()
        .BuildServiceProvider();
}

async System.Threading.Tasks.Task SynchronizationAsync(IServiceProvider serviceProvider)
{
    Log.Information("Synchronization start");
    //ScheduleCreate();

    IEnumerable<SynchronizationModel> fromScanned;
    IEnumerable<EmployeeDataModel> fromEmployees;
    Log.Information("---- Getting service trader connection string ----");
    string ConnectionString = _configuration["ConnectionStrings:Firebird"];
    Log.Debug("Connection string is: {ConnectionString}", ConnectionString);
    Log.Debug("");
    var connection = new FbConnection(ConnectionString);
    try
    {
        await connection.OpenAsync();

        // Getting and comparing employees
        Log.Information("---- Employee synchronization started ----");
        IEnumerable<EmployeeDataModel> dbEmployees = await _employeeRepository.GetEmployeesAsync().ConfigureAwait(false);
        fromEmployees = GetEmployeesFromFirebird(connection);
        if (!(dbEmployees.Count() == fromEmployees.Count()))
        {
            await CompareEmployeesAsync(fromEmployees, dbEmployees);
            Log.Debug("Employee synchronization finished");
        }
        else
            Log.Debug("Employees are up to date.");
        Log.Debug("");


        // Getting and comparing external data
        Log.Information("---- External data synchronization started ----");
        await ExternalDataSynchronizationAsync();
        Log.Debug("External data synchronization finished");
        Log.Debug("");

        // Getting and comparing scanned data
        //Log.Information("---- Used stocks synchronization started ----");
        //fromScanned = GetUsedScannedDataFromFirebird(connection);
        //await _scannedDataRepository.SynchronizationWithServiceTrader(fromScanned);


    }
    catch (Exception x)
    {
        Console.WriteLine(x.Message);
    }
    finally
    {
        connection.Close();
    }
}

IEnumerable<EmployeeDataModel> GetEmployeesFromFirebird(FbConnection conn)
{
    var employeeList = new FbCommand("SELECT WORKER_NAME, WORKER_SURNAME, WORKER_CODE, EMAIL FROM Workers WHERE DISABLED LIKE 'F'", conn);
    var employeeReader = employeeList.ExecuteReader();
    List<EmployeeDataModel> fromEmployees = new();
    while (employeeReader.Read())
    {
        var employee = new EmployeeDataModel
        {
            Name = employeeReader.GetString(0),
            Surname = employeeReader.GetString(1),
            Code = employeeReader.GetString(2),
            Email = employeeReader.GetString(3)
        };

        if (!string.IsNullOrWhiteSpace(employee.Code) && !string.IsNullOrWhiteSpace(employee.Name) && !string.IsNullOrWhiteSpace(employee.Surname))
        {
            if (employee.Code == "ZST" || employee.Code == "JER")
                employee.IsManager = true;

            fromEmployees.Add(employee);
        }
    }
    employeeReader.Close();
    return fromEmployees;
}

async System.Threading.Tasks.Task CompareEmployeesAsync(IEnumerable<EmployeeDataModel> fbEmployees, IEnumerable<EmployeeDataModel> dbEmployees)
{
    List<EmployeeDataModel> toEmployees = new();
    var unmatchedEmployee = fbEmployees.Select(x => new { x.Name, x.Surname, x.Code, x.Email, x.IsManager })
                                       .Except(dbEmployees.Select(y => new { y.Name, y.Surname, y.Code, y.Email, y.IsManager })).ToList();
    Log.Debug("Were found {unmatchedEmployee} employees", unmatchedEmployee.Count);
    foreach (var employee in unmatchedEmployee)
    {
        EmployeeDataModel item = new()
        {
            Name = employee.Name,
            Surname = employee.Surname,
            Code = employee.Code,
            Email = employee.Email,
            IsManager = employee.IsManager,
            Created = DateTime.Now
        };
        
        toEmployees.Add(item);
    }

    foreach (var item in toEmployees)
        await _repository.InsertAsync(item);

    return;
}

IEnumerable<SynchronizationModel> GetUsedScannedDataFromFirebird(FbConnection conn)
{
    List<SynchronizationModel> fromScanned = new();
    var usedList = new FbCommand($@"SELECT TRIM(wr.WORKER_NAME) || ' ' || TRIM(wr.WORKER_SURNAME) as WORKER,
                                        sdc.DOC_SER_NR || '' || sdc.DOC_NR AS DOCNR, gd.BARCODE, sd.ENTERED  
                                    FROM GOODS gd 
                                    JOIN SELL_DET sd ON gd.GOOD_ID = sd.GOOD_ID 
                                    JOIN SELL_DOC sdc ON sd.SELL_DOC_ID = sdc.SELL_DOC_ID
                                    JOIN SELL_DET_WORKERS sdw ON sdw.SELL_DET_ID = sd.SELL_DET_ID  
                                    JOIN WORKERS wr ON wr.WORKER_ID = sdw.WORKERS_ID
                                    WHERE sd.ENTERED > '{DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}'", conn);
    var usedReader = usedList.ExecuteReader();
    while (usedReader.Read())
    {
        var used = new SynchronizationModel
        {
            Employee = usedReader.GetString(0),
            Barcode = usedReader.GetString(2)
        };
        fromScanned.Add(used);
    }

    return fromScanned;
}

async System.Threading.Tasks.Task ExternalDataSynchronizationAsync()
{
    var today = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
    var url = $"?q=createdDate ON_OR_AFTER \"{today:dd.MM.yyyy}\"";
    var nsApiService = new NsRestApiService(_configuration);
    Log.Debug("Getting netsuite items by {date}", $"{today.ToShortDateString()}");
    var netsuiteItems = await nsApiService.GetInventoryItems(url).ConfigureAwait(false);
    Log.Debug("Netsuite items count: {netsuiteItems}", netsuiteItems.Count());
    foreach (var item in netsuiteItems)
    {
        var external = new ExternalDataModel
        {
            Barcode = item.Barcode,
            ItemNumber = item.ItemNumber,
            Name = item.DisplayName,
            PluCode = item.PluCode,
            Unit = item.UnitName.Unit,
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };

        await _externalDataRepository.InsertAsync(external);
    }
}

//static void ScheduleCreate()
//{
//    if (TaskService.Instance.GetTask("StockAccounting synchronization") == null)
//    {
//        string strPath = Path.GetFullPath(Path.Combine(System.AppContext.BaseDirectory, @"Synchronization\StockAccounting.Synchronization.exe"));
//        TaskService ts = new TaskService();
//        TaskDefinition td = ts.NewTask();

//        td.RegistrationInfo.Description = "Synchronization with database from servicetrader";
//        td.Triggers.Add(new DailyTrigger
//        {
//            Enabled = true,
//            StartBoundary = DateTime.Today.AddHours(08),
//            DaysInterval = 1
//        });
//        td.Actions.Add(new ExecAction(strPath));

//        ts.RootFolder.RegisterTaskDefinition("StockAccounting synchronization", td);
//    }
//}

