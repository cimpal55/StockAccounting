using Dayton.NetsuiteOAuth1RestApi.Services;
using LinqToDB;
using LinqToDB.AspNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Core.Data.Utils.ServiceRegistration;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;


IConfiguration _configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  .AddEnvironmentVariables()
  .AddCommandLine(args)
  .Build();

var logFilePath = $"Logs/log-.txt";

#if (RELEASE)
    logFilePath = $"C:\\www\\StockAccounting\\NetsuiteSynchronization\\Logs\\log-.txt";
#endif

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 999)
    .CreateLogger();

var serviceProvider = CreateServices();
var _externalRepository = serviceProvider.GetRequiredService<IGenericRepository<ExternalDataModel>>();
var _externalDataRepository = serviceProvider.GetRequiredService<IExternalDataRepository>();

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

async Task SynchronizationAsync(IServiceProvider serviceProvider)
{
    // Getting and comparing external data
    Log.Information("---- External data synchronization started ----");
    IEnumerable<ExternalDataModel> dbExternalData = await _externalDataRepository.GetExternalDataAsync().ConfigureAwait(false);
    await ExternalDataSynchronizationAsync(dbExternalData.ToList());
    Log.Debug("External data synchronization finished");
    Log.Debug("");
}

async Task ExternalDataSynchronizationAsync(List<ExternalDataModel> dbData)
{
    try
    {
        string[] arguments = Environment.GetCommandLineArgs();

        var today = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        int length = 1;

        if (arguments.Length > length)
        {
            DateTime.TryParse(arguments[length], out var date);
            today = date;
        }

        var url = $"?q=createdDate ON_OR_AFTER \"{today:dd.MM.yyyy}\"";
        var nsApiService = new NsRestApiService(_configuration);
        Log.Debug("Getting netsuite items by {date}", $"{today.ToShortDateString()}");
        var netsuiteItems = await nsApiService.GetInventoryItems(url).ConfigureAwait(false);
        Log.Debug("Netsuite items count: {netsuiteItems}", netsuiteItems.Count());
        List<ExternalDataModel> fbData = new List<ExternalDataModel>();

        string pluCode;

        foreach (var item in netsuiteItems)
        {
            pluCode = item.PluCode == null ? "-" : item.PluCode;

            var external = new ExternalDataModel
            {
                Barcode = item.Barcode,
                ItemNumber = item.ItemNumber,
                Name = item.DisplayName,
                PluCode = pluCode,
                Unit = item.UnitName.Unit,
                Created = DateTime.Now,
                Updated = DateTime.Now,
            };

            fbData.Add(external);
        }

        int unexisted = 0;

        foreach (var item in fbData)
        {
            Log.Debug($"Working with {item}");
            if (_externalDataRepository.CheckIfExists(item.Barcode) != true)
            {
                await _externalRepository
                    .InsertAsync(item)
                    .ConfigureAwait(false);

                unexisted++;
            }
            else
            {
                await _externalDataRepository
                    .UpdateExternalDataAsyncByBarcode(item)
                    .ConfigureAwait(false);
            }
        }

        Log.Debug("Were found {0} unexisted external data", unexisted);
    }
    catch (Exception ex)
    {
        Log.Error(ex.Message);
    }
}