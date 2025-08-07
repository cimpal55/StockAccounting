using StockAccounting.Inventory.ViewModels;

namespace StockAccounting.Inventory.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryDataView
    {
        public InventoryDataView(InventoryDataViewModel vm)
        {
            InitializeComponent();
            OnAppearing();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            await Task.Delay(10);

            documentSearchBar.Focus();
        }
    }
}