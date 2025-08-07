using StockAccounting.Core.Data.Models.Data.StockData;

namespace StockAccounting.Web.ViewModels
{
    public class StockDataViewModel : BaseViewModel
    {
        public IEnumerable<StockDataModel>? StockDataModel { get; set; }

        public int TotalPages { get; set; }

        public int PageIndex { get; set; }

        public int TotalData { get; set; }

        public int PageId { get; set; }
    }
}
