using LinqToDB;
using LinqToDB.AspNet;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Core.Data.Utils.ServiceRegistration;
using StockAccounting.EmailBot;
using StockAccounting.EmailBot.Models;
using StockAccounting.EmailBot.Services;
using StockAccounting.EmailBot.Services.Interfaces;


var logFilePath = $"Logs/log-.txt";

#if (RELEASE)
    logFilePath = $"C:\\www\\StockAccounting\\EmailBot\\Logs\\log-.txt";
#endif

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
    .WriteTo.File($"Logs/log-.txt", rollingInterval: RollingInterval.Month)
    .CreateLogger();

var serviceProvider = CreateServices();
var _employeeRepository = serviceProvider.GetRequiredService<IEmployeeDataRepository>();
var _emailService = serviceProvider.GetRequiredService<IEmailService>();

using var scope = serviceProvider.CreateScope();


SecureSocketOptions _sslOptions = SecureSocketOptions.Auto;

var imapSettings = IMAPSettings.GetIMAPSettings();
using (var client = new IdleClient(imapSettings.Host, imapSettings.Port, _sslOptions, imapSettings.Email, imapSettings.Password, imapSettings.Commands, _employeeRepository, _emailService))
{
    var idleTask = client.RunAsync();

    Task.Run(() =>
    {
        Console.ReadKey(true);
    }).Wait();

    client.Exit();

    idleTask.GetAwaiter().GetResult();
}