using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.Toolkit
{
    [Table(Tables.Toolkits)]
    public record ToolkitBaseModel
    {
        [Column(Columns.Toolkit.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.Toolkit.Name, CanBeNull = false)]
        public string Name { get; set; } = string.Empty;

        [Column(Columns.Toolkit.Description, CanBeNull = true)]
        public string Description { get; set; } = string.Empty;

        [Column(Columns.Toolkit.Barcode, CanBeNull = true)]
        public string Barcode { get; set; } = string.Empty;

        [Column(Columns.Toolkit.TotalQuantity, CanBeNull = true)]
        public double TotalQuantity { get; set; }

        [Column(Columns.Toolkit.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

        [Column(Columns.Toolkit.Updated, CanBeNull = false)]
        public DateTime Updated { get; set; }

    }
}
