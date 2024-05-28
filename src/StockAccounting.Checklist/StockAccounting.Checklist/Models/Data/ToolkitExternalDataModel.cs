using System;
using System.Collections.Generic;
using System.Text;

namespace StockAccounting.Checklist.Models.Data
{
    public class ToolkitExternalDataModel
    {
        public int Id { get; set; }

        public int ExternalDataId { get; set; }

        public int ToolkitId { get; set; }

        public decimal Quantity { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
