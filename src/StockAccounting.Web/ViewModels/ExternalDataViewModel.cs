using StockAccounting.Core.Data.Models.Data;
using System.ComponentModel.DataAnnotations;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Web.ViewModels
{
    public class ExternalDataViewModel : BaseViewModel
    {
        public IEnumerable<ExternalDataModel>? ExternalDataModel { get; set; }

        public IEnumerable<UnitModel>? UnitDataModel { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Unit name is required")]
        public string UnitName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Item number is required")]
        public string ItemNumber { get; set; } = string.Empty;

        [RegularExpression("^(\\d{13})?$", ErrorMessage = "Barcode must be 13 digits long")]
        public string Barcode { get; set; } = string.Empty;

        [Required(ErrorMessage = "PluCode is required")]
        public string PluCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        public int TotalPages { get; set; }

        public int PageIndex { get; set; }

        public int TotalData { get; set; }

        public int PageId { get; set; }

    }
}
