using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Web.ViewModels
{
    public class StockEmployeesViewModel : BaseViewModel
    {
        public IEnumerable<StockEmployeesModel>? StockEmployeesModel { get; set; }

        public int TotalPages { get; set; }

        public int PageIndex { get; set; }

        public int TotalData { get; set; }

        public int PageId { get; set; }
    }
}
