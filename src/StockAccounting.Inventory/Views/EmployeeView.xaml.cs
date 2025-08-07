using StockAccounting.Inventory.ViewModels;

namespace StockAccounting.Inventory.Views
{
    public partial class EmployeeView : ViewBase
    {
        public EmployeeView(EmployeeViewModel vm) 
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}