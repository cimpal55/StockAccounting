using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Web.ViewModels
{
    public class ToolkitExternalViewModel
    {
        public IEnumerable<ToolkitExternalModel>? ToolkitExternalModel { get; set; }

        public int TotalPages { get; set; }

        public int PageIndex { get; set; }

        public int TotalData { get; set; }
    }
}
