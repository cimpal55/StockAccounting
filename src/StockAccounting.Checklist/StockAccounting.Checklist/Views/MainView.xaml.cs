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
            mainViewModel = AppContainer.Resolve<MainViewModel>();

            await Task.Delay(300);

            firstEmployee.Focus();
        }
    }
}