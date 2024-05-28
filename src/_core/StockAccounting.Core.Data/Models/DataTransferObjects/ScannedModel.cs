using StockAccounting.Core.Data.Models.Data;
using System.Collections.ObjectModel;

namespace StockAccounting.Core.Data.Models.DataTransferObjects
{
    public sealed record ScannedModel
    {
        public InventoryDataBaseModel inventoryData { get; set; }

        public ObservableCollection<ExternalDataModel> scannedData { get; set; }

        public ObservableCollection<ToolkitHistoryModel> usedToolkitData { get; set; }

    }
}
