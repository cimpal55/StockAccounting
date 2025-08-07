using LinqToDB.Mapping;

namespace StockAccounting.Core.Data.Models.DataTransferObjects
{
    public class EmployeeDetailsListModel
    {
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
    }
}
