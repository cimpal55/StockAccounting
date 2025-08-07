using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.ToolkitExternal
{
    [Table(Tables.ToolkitExternal)]
    public record ToolkitExternalBaseModel
    {
        [Column(Columns.ToolkitExternal.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.ToolkitExternal.ExternalDataId, CanBeNull = false)]
        public int ExternalDataId { get; set; }

        [Column(Columns.ToolkitExternal.ToolkitId, CanBeNull = false)]
        public int ToolkitId { get; set; }

        [Column(Columns.ToolkitExternal.Quantity, CanBeNull = false)]
        public double Quantity { get; set; } = 1;

        [Column(Columns.ToolkitExternal.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

        [Column(Columns.ToolkitExternal.Updated, CanBeNull = false)]
        public DateTime Updated { get; set; }

    }
}
