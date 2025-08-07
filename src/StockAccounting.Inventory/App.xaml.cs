using StockAccounting.Inventory.Data;

namespace StockAccounting.Inventory
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
