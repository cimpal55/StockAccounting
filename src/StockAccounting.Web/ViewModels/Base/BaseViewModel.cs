namespace StockAccounting.Web.ViewModels
{
    public class BaseViewModel
    {
        public int StartPage { get; set; }

        public int EndPage { get; set; }

        public string SearchText { get; set; } = string.Empty;
    }
}
