using Splat;
using StockAccounting.Checklist.Bootstrap;
using StockAccounting.Checklist.Services.Interfaces;
using StockAccounting.Checklist.ViewModels;
using StockAccounting.Checklist.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StockAccounting.Checklist
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitializeApp();
            MainPage = new NavigationPage(new MainView());
        }
        private async Task InitializeNavigation()
        {
            var navigationService = AppContainer.Resolve<INavigationService>();
            await navigationService.InitializeAsync();
        }

        private void InitializeApp()
        {
            AppContainer.RegisterDependencies();
        }


        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
