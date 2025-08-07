namespace StockAccounting.Core.Data.Models.Data.ToolkitExternal
{
    public record ToolkitExternalModel : ToolkitExternalBaseModel
    {
        public string ExternalDataName { get; set; } = string.Empty;

        public string FormQuantity { get; set; } = string.Empty;
    }
}
