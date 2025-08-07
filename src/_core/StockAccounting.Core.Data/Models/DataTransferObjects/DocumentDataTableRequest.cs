namespace StockAccounting.Core.Data.Models.DataTransferObjects
{
    public class DocumentDataTableRequest
    {
        public int PageId { get; set; }
        public string SearchText { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
    }

}
