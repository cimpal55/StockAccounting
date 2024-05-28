using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Models.Data
{
    [Table(Tables.Toolkits)]
    public record ToolkitBaseModel
    {
        [Column(Toolkit.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Toolkit.Name, CanBeNull = false)]
        public string Name { get; set; } = string.Empty;

        [Column(Toolkit.Description, CanBeNull = true)]
        public string Description { get; set; } = string.Empty;
  
        [Column(Toolkit.Barcode, CanBeNull = true)]
        public string Barcode { get; set; } = string.Empty;

        [Column(Toolkit.TotalQuantity, CanBeNull = true)]
        public double TotalQuantity { get; set; }

        [Column(Toolkit.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

        [Column(Toolkit.Updated, CanBeNull = false)]
        public DateTime Updated { get; set; }

    }
}
