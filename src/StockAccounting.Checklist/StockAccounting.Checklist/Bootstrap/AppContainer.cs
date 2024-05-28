using Autofac;
using Sextant;
using StockAccounting.Checklist.Repositories;
using StockAccounting.Checklist.Repositories.Interfaces;
using StockAccounting.Checklist.Services;
using StockAccounting.Checklist.Services.Interfaces;
using StockAccounting.Checklist.ViewModels;
using System;

namespace StockAccounting.Checklist.Bootstrap
{
    public class AppContainer
    {
        private static IContainer _container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            //ViewModels
            builder.RegisterType<MainViewModel>();

            //services - data
            builder.RegisterType<InventoryDataService>().As<IInventoryDataService>();
            builder.RegisterType<ExternalDataService>().As<IExternalDataService>();
            builder.RegisterType<EmployeeDataService>().As<IEmployeeDataService>();
            builder.RegisterType<ToolkitDataService>().As<IToolkitDataService>();

            //services - general

            builder.RegisterType<DialogService>().As<IDialogService>();

            //General
            builder.RegisterType<GenericRepository>().As<IGenericRepository>();

            _container = builder.Build();
        }

        public static object Resolve(Type typeName)
        {
            return _container.Resolve(typeName);
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
