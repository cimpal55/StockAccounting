using SQLite;

namespace StockAccounting.Core.Android.Models.Data
{
    [Table("tblServerSyncDateTimeData")]
    public sealed class ServerSyncDateTimeDataRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string TableName { get; set; } = string.Empty;

        [NotNull]
        public string LastSyncDateTime { get; set; } = string.Empty;
    }
}
