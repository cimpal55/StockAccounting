using Microsoft.AspNetCore.Cors.Infrastructure;
using StockAccounting.Api.Repositories;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.Utils.ServiceRegistration;

namespace StockAccounting.Api.Utils.ServiceRegistration
{
    public static class ServiceCollectionEx
    {
        public static IServiceCollection AddStockAccountingServices(this IServiceCollection @this) =>
            @this
                .AddStockAccountingRepositories()
                .AddRepositories();

        private static IServiceCollection AddRepositories(this IServiceCollection @this) =>
            @this
                .AddScoped<IExternalDataRepository, ExternalDataRepository>()
                .AddScoped<IEmployeeDataRepository, EmployeeDataRepository>()
                .AddScoped<IDocumentDataRepository, DocumentDataRepository>()
                .AddScoped<IScannedDataRepository, ScannedDataRepository>()
                .AddScoped<IToolkitDataRepository, ToolkitDataRepository>()
                .AddScoped<IInventoryDataRepository, InventoryDataRepository>()
                .AddScoped<IScannedInventoryDataRepository, ScannedInventoryDataRepository>();
    }
}
