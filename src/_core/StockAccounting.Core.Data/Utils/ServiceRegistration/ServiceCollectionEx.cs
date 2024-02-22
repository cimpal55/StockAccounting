using Microsoft.Extensions.DependencyInjection;
using StockAccounting.Core.Data.Repositories;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Core.Data.Services;
using StockAccounting.Core.Data.Services.Interfaces;

namespace StockAccounting.Core.Data.Utils.ServiceRegistration
{
    public static class ServiceCollectionEx
    {
        public static IServiceCollection AddStockAccountingRepositories(this IServiceCollection @this) =>
            @this
                .AddRepositories();

        private static IServiceCollection AddRepositories(this IServiceCollection @this) =>
            @this
                .AddScoped<ISmtpEmailService, SmtpEmailService>()
                .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                .AddScoped<IEmployeeDataRepository, EmployeeDataRepository>()
                .AddScoped<IExternalDataRepository, ExternalDataRepository>()
                .AddScoped<IInventoryDataRepository, InventoryDataRepository>()
                .AddScoped<IScannedDataRepository, ScannedDataRepository>()
                .AddScoped<IStockDataRepository, StockDataRepository>();

        //.AddScoped<IAdministrationRepository, AdministrationRepository>();
    }
}
