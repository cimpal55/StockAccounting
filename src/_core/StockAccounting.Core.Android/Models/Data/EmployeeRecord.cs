using SQLite;

namespace StockAccounting.Core.Android.Models.Data
{
    [Table("tblEmployees")]
    public sealed class EmployeeRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int ExternalId { get; set; }

        [NotNull]
        public string Name { get; set; } = string.Empty;

        [NotNull]
        public string Surname { get; set; } = string.Empty;

        [NotNull] 
        public string Code { get; set; } = string.Empty;

        public DateTime Created { get; set; }
    }
}
