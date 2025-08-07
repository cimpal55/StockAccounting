using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockAccounting.Core.Data.Repositories;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Core.Data.Services;
using StockAccounting.Core.Data.Services.Interfaces;

namespace StockAccounting.UsedStocksSynchronization.Utils.ServiceRegistration
{
    public static class ServiceCollectionEx
    {
        public static IServiceCollection AddUsedStocksSynchronizationRepositories(this IServiceCollection @this, IConfiguration configuration) =>
            @this
                .AddRepositories(configuration);

        private static IServiceCollection AddRepositories(this IServiceCollection @this, IConfiguration configuration) =>
            @this
                .AddScoped<IEmployeeDataRepository, EmployeeDataRepository>()
                .AddScoped<IExternalDataRepository, ExternalDataRepository>()
                .AddScoped<ISmtpEmailService, SmtpEmailService>(
                    serviceProvider => new SmtpEmailService(
                        configuration: configuration,
                        externalDataRepository: serviceProvider.GetRequiredService<IExternalDataRepository>())
                );
    }
}
