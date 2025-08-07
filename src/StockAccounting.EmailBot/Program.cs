using LinqToDB;
using LinqToDB.AspNet;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Core.Data.Utils.ServiceRegistration;
using StockAccounting.EmailBot;
using StockAccounting.EmailBot.Models;
using StockAccounting.EmailBot.Services;
using StockAccounting.EmailBot.Services.Interfaces;
using System.Diagnostics;

var logFilePath = $"EmailBot/Logs/log-.txt";
var isDebug = System.Diagnostics.Debugger.IsAttached;
logFilePath = isDebug
    ? Path.Combine(AppContext.BaseDirectory, "Logs", "log-.txt")
    : @"C:\www\StockAccounting\EmailBot\Logs\log-.txt";

IServiceProvider CreateServices()
{
    return new ServiceCollection()
        .AddLinqToDBContext<AppDataConnection>((provider, options) =>
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

            return options
             .UseSqlServer(connectionString);
        })
        .AddStockAccountingRepositories()
        .AddScoped<IEmailService, EmailService>()
        .BuildServiceProvider();
}

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
    .WriteTo.EventLog("StockAccountingEmailBot", 
        manageEventSource: false,
        restrictedToMinimumLevel: LogEventLevel.Warning)
    .CreateLogger();


var serviceProvider = CreateServices();
var _employeeRepository = serviceProvider.GetRequiredService<IEmployeeDataRepository>();
var _emailService = serviceProvider.GetRequiredService<IEmailService>();

using var scope = serviceProvider.CreateScope();

#if DEBUG
    System.Diagnostics.Debugger.Launch();
#endif

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService();
builder.Services.AddHostedService(
    serviceProvider => 
        new Worker(serviceProvider.GetRequiredService<ILogger<Worker>>(), _emailService, _employeeRepository));
var host = builder.Build();
host.Run();

SecureSocketOptions _sslOptions = SecureSocketOptions.Auto;

//var imapSettings = IMAPSettings.GetIMAPSettings();
//using (var client = new IdleClient(imapSettings.Host, imapSettings.Port, _sslOptions, imapSettings.Email, imapSettings.Password, imapSettings.Commands, _employeeRepository, _emailService))
//{
//    var idleTask = client.RunAsync();

//    Task.Run(() =>
//    {
//        Console.ReadKey(true);
//    }).Wait();

//    client.Exit();

//    idleTask.GetAwaiter().GetResult();
//}