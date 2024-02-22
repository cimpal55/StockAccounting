using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace StockAccounting.Checklist.Models.Data.DataTransferObjects
{
    public class ScannedModel
    {
        public InventoryDataModel inventoryData { get; set; }

        public ObservableCollection<ExternalDataModel> scannedData { get; set; }
    }
}
