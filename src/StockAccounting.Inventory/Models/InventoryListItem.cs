using CommunityToolkit.Mvvm.ComponentModel;
using StockAccounting.Core.Android.Models.Base;

namespace StockAccounting.Inventory.Models
{
    public class InventoryListItem : ListItemBase<int>
    {
        public InventoryListItem(int key)
            : base(key)
        {
        }

        public int ExternalId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Employee1 { get; set; } = string.Empty;

        public string Employee2 { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime? Created { get; set; }
    }
}