namespace StockAccounting.Core.Data.Models.Data
{
    public sealed record ScannedReportExcelModel
    {
        public int Id { get; set; }

        public string Employee { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string ItemNumber { get; set; } = string.Empty;

        public string PluCode { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public DateTime Created { get; set; }

    }
}
