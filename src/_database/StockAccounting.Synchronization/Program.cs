using LinqToDB;
using LinqToDB.AspNet;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Serilog;
using Microsoft.Win32.TaskScheduler;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.EmployeeData;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Core.Data.Utils.ServiceRegistration;

IConfiguration _configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  .AddEnvironmentVariables()
  .AddCommandLine(args)
  .Build();

var logFilePath = $"Logs/log-.txt";

#if (RELEASE)
    logFilePath = $"C:\\www\\StockAccounting\\Synchronization\\Logs\\log-.txt";
#endif

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 999)
    .CreateLogger();

var serviceProvider = CreateServices();
var _repository = serviceProvider.GetRequiredService<IGenericRepository<EmployeeDataModel>>();
var _externalRepository = serviceProvider.GetRequiredService<IGenericRepository<ExternalDataModel>>();
var _externalDataRepository = serviceProvider.GetRequiredService<IExternalDataRepository>();
var _employeeRepository = serviceProvider.GetRequiredService<IEmployeeDataRepository>();
var _scannedDataRepository = serviceProvider.GetRequiredService<IScannedDataRepository>();
var _stockDataRepository = serviceProvider.GetRequiredService<IStockDataRepository>();

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
    var startDate = DateTime.Now;
    Log.Information("------------------------- Synchronization start: {date}", startDate);

    IEnumerable<SynchronizationModel> fromScanned;
    IEnumerable<EmployeeDataModel> fromEmployees;
    Log.Information("---- Getting service trader connection string ----");
    string ConnectionString = _configuration["ConnectionStrings:Firebird"];
    Log.Debug("Connection string is: {ConnectionString}", ConnectionString);
    Log.Debug("");

    using (var connection = new FbConnection(ConnectionString))
    {
        connection.Open();

        //// Getting and comparing employees
        Log.Information("---- Employee synchronization started ----");
        IEnumerable<EmployeeDataModel> dbEmployees = await _employeeRepository.GetEmployeesAsync().ConfigureAwait(false);
        fromEmployees = GetEmployeesFromFirebird(connection);
        if (!(dbEmployees.Count() == fromEmployees.Count()))
        {
            await CompareEmployeesAsync(fromEmployees, dbEmployees, connection);
            Log.Debug("Employee synchronization finished");
        }
        else
            Log.Debug("Employees are up to date.");
        Log.Debug("");


        // Getting and comparing used data
        Log.Information("---- Used stocks synchronization started ----");
        bool stockExists = await _stockDataRepository.CheckIfStockEmployeeExists();
        if (stockExists)
        {
            fromScanned = await GetUsedScannedDataFromFirebirdAsync(connection);
            await _scannedDataRepository.SynchronizationWithServiceTrader(fromScanned);
            Log.Debug("Stock data synchronization finished");
            Log.Debug("");
        }
        else
        {
            Log.Debug("Stock attached to employees weren't found");
        }

        var endDate = DateTime.Now;
        var spentTime = endDate - startDate;
        Log.Information("Synchronization finished: {endDate}", endDate);
        Log.Information("Synchronization spent time: {spentTime}", spentTime);
        Log.Information("");
        Log.Information("");
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
            if (employee.Code == "ZST" || employee.Code == "JER" || employee.Code == "DPE")
                employee.IsManager = true;

            fromEmployees.Add(employee);
        }
    }
    employeeReader.Close();
    return fromEmployees;
}

async System.Threading.Tasks.Task CompareEmployeesAsync(IEnumerable<EmployeeDataModel> fbEmployees, IEnumerable<EmployeeDataModel> dbEmployees, FbConnection conn)
{
    string? email = null;
    List<EmployeeDataModel> toEmployees = new();
    var unmatchedEmployee = fbEmployees.Select(x => new { x.Name, x.Surname, x.Code })
                                       .Except(dbEmployees.Select(y => new { y.Name, y.Surname, y.Code })).ToList();
    
    Log.Debug("Were found {unmatchedEmployee} employees", unmatchedEmployee.Count);
    foreach (var employee in unmatchedEmployee)
    {
        var query = new FbCommand($"SELECT EMAIL FROM Workers WHERE WORKER_CODE = '{employee.Code}'", conn);

        using (var reader = query.ExecuteReader())
        {
            if (reader.Read())
            {
                email = reader.GetString(0);
            }
        }

        EmployeeDataModel item = new()
        {
            Name = employee.Name,
            Surname = employee.Surname,
            Code = employee.Code,
            Email = email,
            IsManager = false,
            Created = DateTime.Now
        };
        
        toEmployees.Add(item);
        Log.Debug("Jauns darbinieks: {0}", employee);
    }

    foreach (var item in toEmployees)
        await _repository.InsertAsync(item);

    return;
}

async Task<IEnumerable<SynchronizationModel>> GetUsedScannedDataFromFirebirdAsync(FbConnection conn)
{
    string[] arguments = Environment.GetCommandLineArgs();
   
    string entered = DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
 
    int length = 1;

    if (arguments.Length > length)
    {
        try
        {
            DateTime.TryParse(arguments[length], out var date);
            entered = date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            #if (RELEASE)
                logFilePath = $"C:\\www\\StockAccounting\\Synchronization\\Logs\\log-{entered}-ConsoleArgument.txt";
            #endif
        }
        catch
        {
            Log.Information("Wrong date format!!!");
            Console.WriteLine("Wrong date format!!!");
        }
    }

    Log.Information("Synchronization after date: {0}", entered);

    List<SynchronizationModel> fromScanned = new();

    var usedList = new FbCommand($@"SELECT wr.WORKER_CODE as WORKER,
                                        sdc.DOC_SER_NR, sdc.DOC_NR, gd.BARCODE, sd.AMOUNT, sd.ENTERED  
                                    FROM GOODS gd 
                                    JOIN SELL_DET sd ON gd.GOOD_ID = sd.GOOD_ID 
                                    JOIN SELL_DOC sdc ON sd.SELL_DOC_ID = sdc.SELL_DOC_ID
                                    JOIN SELL_DET_WORKERS sdw ON sdw.SELL_DET_ID = sd.SELL_DET_ID  
                                    JOIN WORKERS wr ON wr.WORKER_ID = sdw.WORKERS_ID
                                    WHERE CAST(sd.ENTERED AS DATE) = '{entered}'
                                    AND gd.GOOD_TYPE_ID = 3", conn);


    var usedReader = usedList.ExecuteReader();
    while (usedReader.Read())
    {
        var used = new SynchronizationModel
        {
            Employee = usedReader.GetString(0),
            DocumentSerialNumber = usedReader.GetString(1),
            DocumentNumber = usedReader.GetString(2),
            Barcode = usedReader.GetString(3),
            Quantity = usedReader.GetDecimal(4),
            Created = usedReader.GetDateTime(5),
        };

        Log.Debug("{0}", used);

        fromScanned.Add(used);
    }

    return fromScanned;
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

