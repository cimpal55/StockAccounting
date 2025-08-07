using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.InventoryData
{
    [Table(Tables.InventoryData)]
    public class InventoryDataModel
    {
        [Column(Columns.InventoryData.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.InventoryData.Name, CanBeNull = true)]
        public string Name { get; set; } = string.Empty;

        [Column(Columns.InventoryData.Employee1CheckerId, CanBeNull = false)]
        public int Employee1CheckerId { get; set; }

        [Column(Columns.InventoryData.Employee2CheckerId, CanBeNull = true)]
        public int? Employee2CheckerId { get; set; }

        [Column(Columns.InventoryData.ScannedEmployeeId, CanBeNull = false)]
        public int ScannedEmployeeId { get; set; }

        [Column(Columns.InventoryData.Status, CanBeNull = false)]
        public string Status { get; set; } = string.Empty;

        [Column(Columns.InventoryData.ManuallyAdded, CanBeNull = false)]
        public bool ManuallyAdded { get; set; } = false;

        [Column(Columns.InventoryData.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

        [Column(Columns.InventoryData.Updated, CanBeNull = true)]
        public DateTime Updated { get; set; }

        [Column(Columns.InventoryData.Finished, CanBeNull = true)]
        public DateTime? Finished { get; set; }

    }
}