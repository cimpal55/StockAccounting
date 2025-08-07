using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace StockAccounting.Checklist.Models.Data.DataTransferObjects
{
    public class ScannedModel
    {
        public DocumentDataModel documentData { get; set; }

        public ObservableCollection<ExternalDataModel> scannedData { get; set; }

        public ObservableCollection<ToolkitHistoryModel> usedToolkitData { get; set; }
    }
}
