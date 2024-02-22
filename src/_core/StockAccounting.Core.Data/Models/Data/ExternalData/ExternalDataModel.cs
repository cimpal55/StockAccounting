using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Models.Data
{
    [Table(Tables.ExternalData)]
    public sealed record ExternalDataModel
    {
        [Column(ExternalData.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(ExternalData.ItemNumber, CanBeNull = false)]
        public string ItemNumber { get; set; } = string.Empty;

        [Column(ExternalData.Barcode, CanBeNull = false)]
        public string Barcode { get; set; } = string.Empty;

        [Column(ExternalData.PluCode, CanBeNull = false)]
        public string PluCode { get; set; } = string.Empty;

        [Column(ExternalData.Name, CanBeNull = false)]
        public string Name { get; set; } = string.Empty;

        public int UnitId { get; set; }

        [Column(ExternalData.Unit, CanBeNull = true)]
        public string Unit { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        [Column(ExternalData.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

        [Column(ExternalData.Updated, CanBeNull = true)]
        public DateTime Updated { get; set; }
    }
}
