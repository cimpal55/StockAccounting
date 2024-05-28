using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;

namespace StockAccounting.Checklist.Models.Data
{
    public class ExternalDataModel
    {       
        public int Id { get; set; }
        public string ItemNumber { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string PluCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public string FullName
        {
            get { return string.Format("{0}\n({1})", Name, Unit); }
        }
    }

}
