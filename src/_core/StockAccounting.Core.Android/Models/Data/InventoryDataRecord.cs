using SQLite;

namespace StockAccounting.Core.Android.Models.Data
{
    [Table("tblInventoryData")]
    public sealed record InventoryDataRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, Indexed]
        public int ExternalId { get; set; }

        public string Name { get; set; } = string.Empty;

        [NotNull]
        public int Employee1CheckerId { get; set; }

        public int? Employee2CheckerId { get; set; }

        [NotNull]
        public int ScannedEmployeeId { get; set; }

        public bool ManuallyAdded { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime Created { get; set; }

        public DateTime Finished { get; set; }
    }
}
