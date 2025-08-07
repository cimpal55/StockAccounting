using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.Toolkit;
using StockAccounting.Core.Data.Models.Data.ToolkitExternal;
using System.ComponentModel.DataAnnotations;

namespace StockAccounting.Web.ViewModels
{
    public class ToolkitViewModel : BaseViewModel
    {
        public IEnumerable<ToolkitModel>? ToolkitDataModel { get; set; }

        public List<ToolkitExternalModel> ToolkitExternalDataModel { get; set; } = new();

        public List<ExternalDataModel>? ExternalDataModel { get; set; }

        public string ToolkitName { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public decimal TotalQuantity { get; set; }

        public string ExternalDataName { get; set; } = string.Empty;

        public int TotalPages { get; set; }

        public int PageIndex { get; set; }

        public int TotalData { get; set; }

        public int ToolkitExternalDataModelIndex { get => ToolkitExternalDataModel.Count; }
    }
}
