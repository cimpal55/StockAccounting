using System.Text.Json.Serialization;

namespace StockAccounting.Inventory.Models
{
    public sealed class ScannedInventoryDataJson
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("inventoryDataId")]
        public int InventoryDataId { get; set; }

        [JsonPropertyName("pluCode")]
        public string PluCode { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; } = string.Empty;

        [JsonPropertyName("itemNumber")]
        public string ItemNumber { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; } = string.Empty;
    }
}
