using System.Text.Json.Serialization;

namespace StockAccounting.Inventory.Models
{
    public sealed class InventoryDataJson
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("employee1CheckerId")]
        public int Employee1CheckerId { get; set; }

        [JsonPropertyName("employee2CheckerId")]
        public int? Employee2CheckerId { get; set; }

        [JsonPropertyName("scannedEmployeeId")]
        public int ScannedEmployeeId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
