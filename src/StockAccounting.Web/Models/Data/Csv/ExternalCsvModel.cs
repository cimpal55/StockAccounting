using CsvHelper.Configuration.Attributes;

namespace StockAccounting.Core.Data.Repositories.Csv
{
    public record ExternalCsvModel
    {

        [Name("Old product code")]
        public string PluCode { get; set; } = string.Empty;

        [Name("Item Number")]
        public string ItemNumber { get; set; } = string.Empty;

        [Name("LOCAL ITEM NAME TRANSLATION")]
        public string Name { get; set; } = string.Empty;

        [Name("UPC Code")]
        public string Barcode { get; set; } = string.Empty;

        [Name("Primary Units Type")]
        public string Unit { get; set; } = string.Empty;
    }
}
