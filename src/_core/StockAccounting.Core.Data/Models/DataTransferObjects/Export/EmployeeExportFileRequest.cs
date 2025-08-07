namespace StockAccounting.Core.Data.Models.DataTransferObjects.Export
{
    public sealed record EmployeeExportFileRequest
    {
        public string Format { get; init; } = string.Empty;

        public string DocType { get; init; } = string.Empty;
    }
}
