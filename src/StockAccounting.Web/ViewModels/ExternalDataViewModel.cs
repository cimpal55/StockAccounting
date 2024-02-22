using StockAccounting.Core.Data.Models.Data;
using System.ComponentModel.DataAnnotations;

namespace StockAccounting.Web.ViewModels
{
    public class ExternalDataViewModel : BaseViewModel
    {
        public IEnumerable<ExternalDataModel>? ExternalDataModel { get; set; }

        public int TotalPages { get; set; }

        public int PageIndex { get; set; }

        public int TotalData { get; set; }
    }
}
