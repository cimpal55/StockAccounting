using System;
using System.Collections.Generic;
using System.Text;

namespace StockAccounting.Checklist.Models.Data
{
    public class ScannedStockDataModel
    {
        public int Id { get; set; }
        
        public int InventoryDataId { get; set; }

        public int ExternalDataId { get; set; }

        public int Quantity { get; set; }
        
        public DateTime Created { get; set; }
    }
}
