using CommunityToolkit.Mvvm.ComponentModel;

namespace StockAccounting.Inventory.Models
{
    public sealed class EmployeeListItem : ObservableObject
    {
        public EmployeeListItem(int key)
        {
        }

        public int DisplayNameId { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
    }
}
