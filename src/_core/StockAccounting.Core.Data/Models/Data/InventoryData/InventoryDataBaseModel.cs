using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Models.Data
{
    [Table(Tables.InventoryData)]
    public record InventoryDataBaseModel
    {
        [Column(InventoryData.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(InventoryData.Employee1Id, CanBeNull = false)]
        public int Employee1Id { get; set; }

        [Column(InventoryData.Employee2Id, CanBeNull = false)]
        public int Employee2Id { get; set; }

        [Column(InventoryData.ManuallyAdded, CanBeNull = false)]
        public bool ManuallyAdded { get; set; } = false;

        [Column(InventoryData.IsSynchronization, CanBeNull = false)]
        public bool IsSynchronization { get; set; }

        [Column(InventoryData.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

        [Column(InventoryData.Updated, CanBeNull = true)]
        public DateTime Updated { get; set; }
    }
}
