namespace StockAccounting.Web.Services.Interfaces
{
    public interface IFileImportService
    {
        Task ImportNetSuiteCsvData(StreamReader stream);
    }
}
