using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StockAccounting.Checklist.Models.Data
{
    public class EmployeeDataModel
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("surname")]
        public string Surname { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        public string FullName
        {
            get { return string.Format("{0} {1} {2}", Name, Surname, Code); }
        }

    }
}
