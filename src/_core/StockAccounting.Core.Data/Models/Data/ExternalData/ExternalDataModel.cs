using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.ExternalData
{
    [Table(Tables.ExternalData)]
    public sealed record ExternalDataModel
    {
        [Column(Columns.ExternalData.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.ExternalData.ItemNumber, CanBeNull = false)]
        public string ItemNumber { get; set; } = string.Empty;

        [Column(Columns.ExternalData.Barcode, CanBeNull = false)]
        public string Barcode { get; set; } = string.Empty;

        [Column(Columns.ExternalData.PluCode, CanBeNull = false)]
        public string PluCode { get; set; } = string.Empty;

        [Column(Columns.ExternalData.Name, CanBeNull = false)]
        public string Name { get; set; } = string.Empty;

        //public int UnitId { get; set; }

        [Column(Columns.ExternalData.Unit, CanBeNull = true)]
        public string Unit { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public string Document { get; set; } = string.Empty;


        [Column(Columns.ExternalData.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

        [Column(Columns.ExternalData.Updated, CanBeNull = true)]
        public DateTime Updated { get; set; }
    }
}
