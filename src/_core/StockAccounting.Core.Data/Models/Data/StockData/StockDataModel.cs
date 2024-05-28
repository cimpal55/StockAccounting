using LinqToDB.Mapping;

namespace StockAccounting.Core.Data.Models.Data
{
    public sealed record StockDataModel : StockDataBaseModel
    {
        [Column(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Column(Name = "ItemNumber")]
        public string ItemNumber { get; set; } = string.Empty;

        [Column(Name = "PluCode")]
        public string PluCode { get; set; } = string.Empty;

        [Column(Name = "Barcode")]
        public string Barcode { get; set; } = string.Empty;

        [Column(Name = "Employee")]
        public string Employee { get; set; } = string.Empty;

        [Column(Name = "EmployeeId")]
        public int EmployeeId { get; set; }

        [Column(Name = "Type")]
        public string Type { get; set; } = string.Empty;

        [Column(Name = "Unit")]
        public string Unit { get; set; } = string.Empty;

        [Column(Name = "LeftQuantity")]
        public decimal LeftQuantity { get; set; }

    }
}
