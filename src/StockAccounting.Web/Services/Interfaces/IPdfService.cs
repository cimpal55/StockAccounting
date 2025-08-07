using QuestPDF.Infrastructure;
using StockAccounting.Core.Data.Models.Data.StockData;

namespace StockAccounting.Web.Services.Interfaces
{
    public interface IPdfService
    {
        public Task<IDocument> CreateEmployeePdf(IEnumerable<StockDataModel> data, string docType);
    }
}
