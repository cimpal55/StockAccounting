namespace StockAccounting.Core.Data.Models.DataTransferObjects
{
    public sealed record SynchronizationModel
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string Employee { get; set; } = string.Empty;

    }
}
