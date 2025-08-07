using StockAccounting.Inventory.Views;

namespace StockAccounting.Inventory
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("EmployeeView", typeof(EmployeeView));
            Routing.RegisterRoute("InventoryView", typeof(InventoryDataView));
            Routing.RegisterRoute("ScannedView", typeof(ScannedInventoryDataView));
            Routing.RegisterRoute("ScannedAddView", typeof(ScannedInventoryDataAddView));
        }
    }
}
