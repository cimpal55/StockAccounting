namespace StockAccounting.Core.Data.Models.Data.DocumentData
{
    public sealed record DocumentDataModel : DocumentDataBaseModel
    {
        public string Employee1 { get; set; } = string.Empty;

        public string Employee2 { get; set; } = string.Empty;
    }
}
