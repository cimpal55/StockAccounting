using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Web.ViewModels
{
    public class EmployeeDetailsViewModel : BaseViewModel
    {
        public IEnumerable<EmployeeDetailsListModel>? EmployeeDetailsModel { get; set; }

        public int TotalPages { get; set; }

        public int PageIndex { get; set; }

        public int TotalData { get; set; }

        public int PageId { get; set; }
    }
}