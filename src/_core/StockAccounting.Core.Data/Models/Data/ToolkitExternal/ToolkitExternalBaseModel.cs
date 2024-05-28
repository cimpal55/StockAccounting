using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Models.Data
{
    [Table(Tables.ToolkitExternal)]
    public record ToolkitExternalBaseModel
    {
        [Column(ToolkitExternal.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(ToolkitExternal.ExternalDataId, CanBeNull = false)]
        public int ExternalDataId { get; set; }

        [Column(ToolkitExternal.ToolkitId, CanBeNull = false)]
        public int ToolkitId { get; set; }

        [Column(ToolkitExternal.Quantity, CanBeNull = false)]
        public double Quantity { get; set; } = 1;

        [Column(ToolkitExternal.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

        [Column(ToolkitExternal.Updated, CanBeNull = false)]
        public DateTime Updated { get; set; }

    }
}
