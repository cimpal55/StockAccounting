using StockAccounting.Core.Data.Models.Data.ExternalData;

namespace StockAccounting.Api.Repositories.Interfaces
{
    public interface IExternalDataRepository
    {
        Task<List<ExternalDataModel>> GetExternalData();
        Task<ExternalDataModel> GetExternalDataByBarcode(string plucode);
        Task<ExternalDataModel> GetExternalDataById(int externalDataId);
    }
}
