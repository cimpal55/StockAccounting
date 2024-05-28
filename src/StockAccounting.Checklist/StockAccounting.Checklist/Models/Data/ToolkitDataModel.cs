using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockAccounting.Checklist.Models.Data
{
    public class ToolkitDataModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("barcode")]
        public string Barcode { get; set; } = string.Empty;

        [JsonProperty("totalquantity")]
        public double TotalQuantity { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("updated")]
        public DateTime Updated { get; set; }
    }
}
