using StockAccounting.Inventory.ViewModels;

namespace StockAccounting.Inventory.Views
{
    public partial class MainView : ViewBase
    {
        public MainView(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }

}
