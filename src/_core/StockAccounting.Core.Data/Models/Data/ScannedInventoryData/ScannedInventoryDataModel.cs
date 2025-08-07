using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.ScannedInventoryData
{
    [Table(Tables.ScannedInventoryData)]
    public sealed class ScannedInventoryDataModel
    {
        [Column(Columns.ScannedInventoryData.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.ScannedInventoryData.InventoryDataId, CanBeNull = false)]
        public int InventoryDataId { get; set; }

        [Column(Columns.ScannedInventoryData.ItemNumber), NotNull]
        public string ItemNumber { get; set; } = string.Empty;

        [Column(Columns.ScannedInventoryData.Name, CanBeNull = false)]
        public string Name { get; set; } = string.Empty;

        [Column(Columns.ScannedInventoryData.Barcode, CanBeNull = false)]
        public string Barcode { get; set; } = string.Empty;

        [Column(Columns.ScannedInventoryData.PluCode, CanBeNull = false)]
        public string PluCode { get; set; } = string.Empty;

        [Column(Columns.ScannedInventoryData.Quantity, CanBeNull = false)]
        public decimal Quantity { get; set; }

        [Column(Columns.ScannedInventoryData.CheckedQuantity, CanBeNull = false)]
        public decimal CheckedQuantity { get; set; }

        [Column(Columns.ScannedInventoryData.FinalQuantity, CanBeNull = false)]
        public decimal FinalQuantity { get; set; }

        [Column(Columns.ScannedInventoryData.IsExternal, CanBeNull = false)]
        public bool IsExternal { get; set; } = false;

        [Column(Columns.ScannedInventoryData.Unit, CanBeNull = false)]
        public string Unit { get; set; } = string.Empty;

        [Column(Columns.ScannedInventoryData.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

        [Column(Columns.ScannedInventoryData.Updated, CanBeNull = true)]
        public DateTime Updated { get; set; }
    }
}
