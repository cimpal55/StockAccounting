using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Models.Data
{
    [Table(Tables.Unit)]
    public record UnitModel
    {
        [Column(Unit.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Unit.Name, CanBeNull = false)]
        public string Name { get; set; } = string.Empty;

        [Column(Unit.Created, CanBeNull = false)]
        public DateTime Created { get; set; }
    }
}
