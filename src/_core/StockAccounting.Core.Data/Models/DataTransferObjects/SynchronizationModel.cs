using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Core.Data.Models.DataTransferObjects
{
    public sealed record SynchronizationModel
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string DocumentSerialNumber { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string Employee { get; set; } = string.Empty;
        public string ReturnEmployee { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public DateTime Created { get; set; }
        public DateTime DocumentCreated { get; set; }
        public ExternalDataModel ExternalData { get; set; }
    }
}
