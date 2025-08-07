using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.EmailBot.Models;
using StockAccounting.EmailBot.Services.Interfaces;

namespace StockAccounting.EmailBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmailService _emailService;
        private readonly IEmployeeDataRepository _employeeRepository;

        public Worker(ILogger<Worker> logger, 
            IEmailService emailService,
            IEmployeeDataRepository employeeRepository)
        {
            _logger = logger;
            _emailService = emailService;
            _employeeRepository = employeeRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                SecureSocketOptions _sslOptions = SecureSocketOptions.Auto;

                var imapSettings = IMAPSettings.GetIMAPSettings();
                using (var client = new IdleClient(imapSettings.Host, imapSettings.Port, _sslOptions,
                           imapSettings.Email, imapSettings.Password, imapSettings.Commands,
                           _employeeRepository, _emailService))
                {
                    var idleTask = client.RunAsync();

                    Task.Run(() =>
                    {
                        Thread.Sleep(Timeout.Infinite);
                    }).Wait();

                    client.Exit();

                    idleTask.GetAwaiter().GetResult();
                }
            }
        }
    }
}