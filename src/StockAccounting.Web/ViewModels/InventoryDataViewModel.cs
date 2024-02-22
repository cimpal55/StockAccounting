using StockAccounting.Core.Data.Models.Data;
using System.ComponentModel.DataAnnotations;

namespace StockAccounting.Web.ViewModels
{
    public class InventoryDataViewModel : BaseViewModel
    {
        public IEnumerable<InventoryDataModel>? InventoryDataModel { get; set; }

        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "This field is required!")]
        public int Employee1Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "This field is required!")]
        public int Employee2Id { get; set; }

        public IEnumerable<EmployeeDataModel>? Employees { get; set; }

        public IEnumerable<ExternalDataModel>? ExternalData { get; set; }

        public int TotalPages { get; set; }

        public int PageIndex { get; set; }

        public int TotalData { get; set; }

        public int PageId { get; set; }

    }
}
