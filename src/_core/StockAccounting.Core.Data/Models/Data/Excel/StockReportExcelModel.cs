namespace StockAccounting.Core.Data.Models.Data.Excel
{
    public class StockReportExcelModel
    {
        public int Id { get; set; }

        public string DocumentNumber { get; set; } = string.Empty;

        public string Employee { get; set; } = string.Empty;

        public string StockName { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Barcode { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public DateTime Created { get; set; }
    }
}
