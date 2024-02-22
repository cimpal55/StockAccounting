using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Core.Data.Services.Interfaces
{
    public interface ISmtpEmailService
    {
        void SendEmail(string email, List<ScannedDataBaseModel> stocksList);
        List<ScannedDataModel> GetFinishedListForHtml(List<ScannedDataBaseModel> stocksList);
        string GetHtmlForEmailNotification(List<ScannedDataModel> stocksList);
    }
}
