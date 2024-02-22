namespace StockAccounting.Core.Data.Models.Data
{
    public sealed record InventoryDataModel : InventoryDataBaseModel
    {
        public string Employee1 { get; set; } = string.Empty;

        public string Employee2 { get; set; } = string.Empty;
    }
}
