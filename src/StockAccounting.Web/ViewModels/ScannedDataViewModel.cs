using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using System.ComponentModel.DataAnnotations;

namespace StockAccounting.Web.ViewModels
{
    public class ScannedDataViewModel : BaseViewModel
    {
        public IEnumerable<ScannedDataModel>? ScannedDataModel { get; set; }

        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "This field is required!")]
        [Required(ErrorMessage = "External data is required!")]
        public int ExternalDataId { get; set; }

        public int DocumentDataId { get; set; }

        public decimal Quantity { get; set; }

        [Range(0.01, int.MaxValue, ErrorMessage = "This field is required!")]
        [Required(ErrorMessage = "Quantity is required!")]
        public string QuantityString { get; set; } = string.Empty;

        public IEnumerable<ExternalDataModel>? ExternalData { get; set; }

        public int TotalPages { get; set; }

        public int PageIndex { get; set; }

        public int TotalData { get; set; }

        public int PageId { get; set; }
    }
}
