using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.ScannedData
{
    [Table(Tables.ScannedData)]
    public record ScannedDataBaseModel
    {
        [Column(Columns.ScannedData.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.ScannedData.DocumentSerialNumber, CanBeNull = false)]
        public string DocumentSerialNumber { get; set; } = string.Empty;

        [Column(Columns.ScannedData.DocumentNumber, CanBeNull = false)]
        public int? DocumentNumber { get; set; }

        [Column(Columns.ScannedData.DocumentDataId, CanBeNull = true)]
        public int DocumentDataId { get; set; }

        [Column(Columns.ScannedData.InventoryDataId, CanBeNull = true)]
        public int? InventoryDataId { get; set; }

        [Column(Columns.ScannedData.ExternalDataId, CanBeNull = false)]
        public int ExternalDataId { get; set; }

        [Column(Columns.ScannedData.Quantity, CanBeNull = false)]
        public decimal Quantity { get; set; }

        [Column(Columns.ScannedData.Created, CanBeNull = false)]
        public DateTime Created { get; set; }
    }
}
