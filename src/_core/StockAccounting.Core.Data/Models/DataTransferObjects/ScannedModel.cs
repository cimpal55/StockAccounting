using System.Collections.ObjectModel;
using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.ToolkitHistory;

namespace StockAccounting.Core.Data.Models.DataTransferObjects
{
    public sealed record ScannedModel
    {
        public DocumentDataBaseModel documentData { get; set; }

        public ObservableCollection<ExternalDataModel> scannedData { get; set; }

        public ObservableCollection<ToolkitHistoryModel> usedToolkitData { get; set; }

    }
}
