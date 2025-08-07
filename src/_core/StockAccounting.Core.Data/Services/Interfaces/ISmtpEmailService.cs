using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Core.Data.Services.Interfaces
{
    public interface ISmtpEmailService
    {
        void SendEmail(string email, List<ScannedDataBaseModel> stocksList);
        void SendEmail(IEnumerable<IEnumerable<SynchronizationModel>> stocksList, DateTime date);
        List<ScannedDataModel> GetFinishedListForHtml(List<ScannedDataBaseModel> stocksList);
        string GetHtmlForEmailNotification(List<ScannedDataModel> stocksList);
        string GetHtmlForEmailNotification(List<ExternalDataModel> stocksList);
    }
}
