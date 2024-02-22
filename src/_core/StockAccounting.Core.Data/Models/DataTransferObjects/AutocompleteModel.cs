namespace StockAccounting.Core.Data.Models.DataTransferObjects
{
    public sealed record AutocompleteModel
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
