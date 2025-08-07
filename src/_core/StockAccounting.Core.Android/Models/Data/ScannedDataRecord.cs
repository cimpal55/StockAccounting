using SQLite;

namespace StockAccounting.Core.Android.Models.Data
{
    [Table("tblScannedData")]
    public sealed record ScannedInventoryDataRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int ExternalId { get; set; }

        [NotNull]
        public string Name { get; set; } = string.Empty;

        [NotNull]
        public string Barcode { get; set; } = string.Empty;

        [NotNull]
        public string PluCode { get; set; } = string.Empty;

        public string ItemNumber { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal TotalCheckedQuantity { get; set; }

        public decimal CheckedQuantity { get; set; }

        public decimal FinalQuantity { get; set; }

        public bool IsExternal { get; set; }

        [NotNull, Indexed]
        public int InventoryDataId { get; set; }

        [NotNull]
        public bool IsLocallyAdded { get; set; } = false;

        public bool IsQuantitiesEqual => Quantity == TotalCheckedQuantity;

        public string Unit { get; set; } = string.Empty;

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
