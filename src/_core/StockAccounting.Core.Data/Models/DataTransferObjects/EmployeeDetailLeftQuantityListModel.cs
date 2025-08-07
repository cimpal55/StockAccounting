using LinqToDB.Mapping;

namespace StockAccounting.Core.Data.Models.DataTransferObjects
{
    public class EmployeeDetailLeftQuantityListModel
    {
        [Column("DocumentSerialNumber")]
        public string DocumentSerialNumber { get; set; } = string.Empty;

        [Column("DocumentNumber")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Barcode")]
        public string Barcode { get; set; } = string.Empty;

        [Column("ItemNumber")]
        public string ItemNumber { get; set; } = string.Empty;

        [Column("PluCode")]
        public string PluCode { get; set; } = string.Empty;

        [Column("Quantity")]
        public decimal Quantity { get; set; }

        [Column("Employee")]
        public string Employee { get; set; } = string.Empty;

        [Column("EmployeeId")]
        public int EmployeeId { get; set; }

        [Column("ExternalDataId")]
        public int ExternalDataId { get; set; }

        [Column("Type")]
        public string StockType { get; set; } = string.Empty;

        [Column("Created")]
        public DateTime Created { get; set; }

    }
}