using StockAccounting.Core.Data.Models.Data;
using System.ComponentModel.DataAnnotations;

namespace StockAccounting.Web.ViewModels
{
    public class ScannedDataViewModel : BaseViewModel
    {
        public IEnumerable<ScannedDataModel>? ScannedDataModel { get; set; }

        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "This field is required!")]
        public int ExternalDataId { get; set; }

        public int InventoryDataId { get; set; }

        [Range(0.01, int.MaxValue, ErrorMessage = "This field is required!")]
        public decimal Quantity { get; set; }

        public IEnumerable<ExternalDataModel>? ExternalData { get; set; }

        public int TotalPages { get; set; }

        public int PageIndex { get; set; }

        public int TotalData { get; set; }

        public int PageId { get; set; }
    }
}
