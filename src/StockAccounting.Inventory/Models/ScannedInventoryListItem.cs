using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using StockAccounting.Core.Android.Models.Base;

namespace StockAccounting.Inventory.Models
{
    public partial class ScannedInventoryListItem : ReactiveObject
    {
        public int Key { get; }

        public ScannedInventoryListItem(int key)
        {
            Key = key;
        }

        private decimal _quantity;

        private decimal _totalCheckedQuantity;

        public int Nr { get; set; }

        public int Id { get; set; }

        public int ExternalId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Barcode { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public decimal TotalCheckedQuantity
        {
            get => _totalCheckedQuantity;
            set
            {
                this.RaiseAndSetIfChanged(ref _totalCheckedQuantity, value);
                this.RaisePropertyChanged(nameof(IsQuantitiesEqual));
            }
        }

        private DateTime _updated;
        public DateTime Updated
        {
            get => _updated;
            set => this.RaiseAndSetIfChanged(ref _updated, value);
        }

        private decimal _checkedQuantity;
        public decimal CheckedQuantity
        {
            get => _checkedQuantity;
            set => this.RaiseAndSetIfChanged(ref _checkedQuantity, value);
        }

        public decimal DifferenceQuantity { get; set; }
        public string ItemNumber { get; set; }

        public string Unit { get; set; } = string.Empty;

        public bool IsLocallyAdded { get; set; } = false;
        public bool IsQuantitiesEqual => Amount == TotalCheckedQuantity;
    }
}