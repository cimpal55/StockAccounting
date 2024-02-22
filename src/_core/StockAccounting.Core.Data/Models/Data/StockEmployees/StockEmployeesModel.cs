using LinqToDB.Mapping;

namespace StockAccounting.Core.Data.Models.Data
{
    public sealed record StockEmployeesModel : StockEmployeesBaseModel
    {
        [Column("Employee")]
        public string Employee { get; set; } = string.Empty;

        [Column("Type")]
        public string StockType { get; set; } = string.Empty;

        [Column("StockName")]
        public string StockName { get; set; } = string.Empty;

        public int ExternalDataId { get; set; }

        public bool IsSynchronization { get; set; }

    }
}
