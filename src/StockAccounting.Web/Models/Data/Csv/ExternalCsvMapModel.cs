using CsvHelper.Configuration;

namespace StockAccounting.Core.Data.Repositories.Csv
{
    public sealed class ExternalCsvMapModel : ClassMap<ExternalCsvModel>
    {
        public ExternalCsvMapModel()
        {
            Map(m => m.PluCode).Name("Old product code");
            Map(m => m.ItemNumber).Name("Item Number");
            Map(m => m.Barcode).Name("UPC Code");
            Map(m => m.Name).Name("LOCAL ITEM NAME TRANSLATION");
            Map(m => m.Unit).Name("Primary Units Type");
        }
    }
}
