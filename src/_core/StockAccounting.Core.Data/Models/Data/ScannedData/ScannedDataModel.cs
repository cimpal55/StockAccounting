using LinqToDB.Mapping;

namespace StockAccounting.Core.Data.Models.Data
{
    public sealed record ScannedDataModel : ScannedDataBaseModel
    {
        [Column(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Column(Name = "ItemNumber")]
        public string ItemNumber { get; set; } = string.Empty;

        [Column(Name = "PluCode")]
        public string PluCode { get; set; } = string.Empty;

    }
}
