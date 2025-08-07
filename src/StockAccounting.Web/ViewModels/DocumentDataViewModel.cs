using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.Data.EmployeeData;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using System.ComponentModel.DataAnnotations;

namespace StockAccounting.Web.ViewModels
{
    public class DocumentDataViewModel : BaseViewModel
    {
        public IEnumerable<DocumentDataModel>? DocumentDataModel { get; set; }

        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "This field is required!")]
        public int Employee1Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "This field is required!")]
        public int Employee2Id { get; set; }

        [Required(ErrorMessage = "This field is required!")]
        public DateTime Created { get; set; }

        public IEnumerable<EmployeeDataModel>? Employees { get; set; }

        public IEnumerable<ExternalDataModel>? ExternalData { get; set; }

        public int TotalPages { get; set; }

        public int PageIndex { get; set; }

        public int TotalData { get; set; }

        public int PageId { get; set; }

        public string SortColumn { get; set; } = string.Empty;

        public string SortDirection { get; set; } = string.Empty;

    }
}
