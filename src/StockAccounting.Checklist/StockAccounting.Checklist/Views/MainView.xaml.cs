using Acr.UserDialogs;
using StockAccounting.Checklist.Bootstrap;
using StockAccounting.Checklist.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StockAccounting.Checklist.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView : ContentPage
    {
        MainViewModel mainViewModel;
        public MainView()
        {
            InitializeComponent();
            OnAppearing();
        }
        protected override async void OnAppearing()
        {
            AppContainer.Resolve<MainViewModel>();

            //mainViewModel.OnAppearing();

            await Task.Delay(10);

            firstEmployee.Focus();
        }
    }
}