using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace StockAccounting.Core.Android.Models.Data
{
    [Table("tblExternalData")]
    public partial class ExternalDataRecord : ObservableObject
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string ItemNumber { get; set; } = string.Empty;

        public string PluCode { get; set; } = string.Empty;

        public string Barcode { get; set; } = string.Empty;

        [ObservableProperty] private decimal _checkedQuantity;

        [ObservableProperty] [property: Ignore] private DateTime _updated;
        public string FullName
        {
            get { return string.Format("{0}\n({1})", Name, Unit); }
        }
    }
}
