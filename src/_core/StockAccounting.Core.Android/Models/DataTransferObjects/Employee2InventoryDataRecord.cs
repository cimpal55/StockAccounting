using SQLite;

namespace StockAccounting.Core.Android.Models.DataTransferObjects
{
    [Table("tblEmployees2InventoryData")]
    public sealed class Employee2InventoryDataRecord
    {
        [NotNull]
        public int Employee1Id { get; set; }

        public int? Employee2Id { get; set; }

        [NotNull]
        public int ScannedEmployeeId { get; set; }

        public int? InventDataId { get; set; }
    }
}
