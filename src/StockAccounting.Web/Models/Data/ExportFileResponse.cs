using StockAccounting.Web.Extensions;

namespace StockAccounting.Web.Models.Data
{
    public sealed record ExportFileResponse(Stream FileStream)
    {
        public string ContentType { get; init; } = string.Empty;

        public string? FileName { get; init; }
    }
}
