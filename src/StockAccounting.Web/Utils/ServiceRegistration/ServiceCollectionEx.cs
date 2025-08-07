using StockAccounting.Core.Data.Utils.ServiceRegistration;
using StockAccounting.Web.Repositories;
using StockAccounting.Web.Repositories.Interfaces;
using StockAccounting.Web.Services;
using StockAccounting.Web.Services.Interfaces;

namespace StockAccounting.Web.Utils.ServiceRegistration
{
    public static class ServiceCollectionEx
    {
        public static IServiceCollection AddStockAccountingServices(this IServiceCollection @this) =>
            @this
                .AddServices()
                .AddStockAccountingRepositories();

        private static IServiceCollection AddServices(this IServiceCollection @this) =>
            @this
                .AddScoped<IDataRepository, DataRepository>()
                .AddScoped<IPaginationService, PaginationService>()
                .AddScoped<IFileImportService, FileImportService>()
                .AddScoped<IFileUploadService, FileUploadService>()
                .AddScoped<IFileExportService, FileExportService>();

        //.AddScoped<IAdministrationRepository, AdministrationRepository>();
    }
}
