using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Models.Data
{
    [Table(Tables.ScannedData)]
    public record ScannedDataBaseModel
    {
        [Column(ScannedData.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(ScannedData.DocumentSerialNumber, CanBeNull = false)]
        public string DocumentSerialNumber { get; set; } = string.Empty;

        [Column(ScannedData.DocumentNumber, CanBeNull = false)]
        public int? DocumentNumber { get; set; }

        [Column(ScannedData.InventoryDataId, CanBeNull = false)]
        public int InventoryDataId { get; set; }

        [Column(ScannedData.ExternalDataId, CanBeNull = false)]
        public int ExternalDataId { get; set; }

        [Column(ScannedData.Quantity, CanBeNull = false)]
        public decimal Quantity { get; set; }

        [Column(ScannedData.Created, CanBeNull = false)]
        public DateTime Created { get; set; }
    }
}
