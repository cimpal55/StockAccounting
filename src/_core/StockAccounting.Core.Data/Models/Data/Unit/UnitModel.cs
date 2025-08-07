using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.Unit
{
    [Table(Tables.Unit)]
    public record UnitModel
    {
        [Column(Columns.Unit.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.Unit.Name, CanBeNull = false)]
        public string Name { get; set; } = string.Empty;

        [Column(Columns.Unit.Created, CanBeNull = false)]
        public DateTime Created { get; set; }
    }
}
