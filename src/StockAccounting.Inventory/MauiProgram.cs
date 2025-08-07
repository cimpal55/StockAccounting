using Acr.UserDialogs;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using StockAccounting.Core.Android.Data;
using StockAccounting.Core.Android.Services;
using StockAccounting.Core.Android.Services.Interfaces;
using StockAccounting.Core.Data.Repositories;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Inventory.Data;
using StockAccounting.Inventory.Repositories;
using StockAccounting.Inventory.Repositories.Interfaces;
using StockAccounting.Inventory.Services;
using StockAccounting.Inventory.Services.Interfaces;
using StockAccounting.Inventory.Utility;
using StockAccounting.Inventory.ViewModels;
using StockAccounting.Inventory.Views;

namespace StockAccounting.Inventory
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
#if ANDROID
            UserDialogs.Init(() => Platform.CurrentActivity);
#endif  

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .RegisterRepositories()
                .RegisterServices()
                .RegisterViewModels()
                .RegisterViews();

#if ANDROID
            builder.Services.AddSingleton(UserDialogs.Instance);

            Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping("NoIcon", (handler, view) =>
            {
                var searchView = handler.PlatformView;

                var searchIcon = searchView.FindViewById(Resource.Id.search_mag_icon);
                if (searchIcon != null)
                {
                    searchIcon.Visibility = Android.Views.ViewStates.Gone;
                }
            });
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            app.Services
                .GetRequiredService<IDatabaseInitService>()
                .InitSchema();

            app.Services
                .GetRequiredService<IConfiguration>()
                .SetPreferences();

            return app;
        }


        private static MauiAppBuilder RegisterRepositories(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<DatabaseContext>();
            builder.Services.AddTransient<IScannedInventoryDataRepository, ScannedInventoryDataRepository>();
            builder.Services.AddTransient<IInventoryDataRepository, InventoryDataRepository>();
            builder.Services.AddTransient<IAdministrationRepository, AdministrationRepository>();

            return builder;
        }

        private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IDatabaseInitService, DatabaseInitService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IRestService, RestService>();
            builder.Services.AddSingleton<IConfiguration, ConfigurationService>();
            builder.Services.AddSingleton<IServerService, ServerService>();

            return builder;
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<EmployeeViewModel>();
            builder.Services.AddTransient<InventoryDataViewModel>();
            builder.Services.AddTransient<ScannedInventoryDataViewModel>();
            builder.Services.AddTransient<ScannedInventoryDataAddViewModel>();

            return builder;
        }

        private static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<MainView>();
            builder.Services.AddTransient<EmployeeView>();
            builder.Services.AddTransient<InventoryDataView>();
            builder.Services.AddTransient<ScannedInventoryDataView>();
            builder.Services.AddTransient<ScannedInventoryDataAddView>();

            return builder;
        }
    }
}
