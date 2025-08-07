namespace StockAccounting.Core.Data.Models.DataTransferObjects.Export
{
    public sealed record EmployeeExportFileResponse(Stream FileStream)
    {
        public string ContentType { get; init; } = string.Empty;

        public string FileName { get; init; } = string.Empty;
    }
}
