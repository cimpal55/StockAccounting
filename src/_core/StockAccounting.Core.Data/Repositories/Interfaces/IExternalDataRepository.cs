using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IExternalDataRepository
    {
        Task<IEnumerable<ExternalDataModel>> GetExternalDataAsync();
        IQueryable<ExternalDataModel> GetExternalDataQueryable();
        IQueryable<ExternalDataModel> GetExternalDataSearchTextQueryable(string searchText);
        ExternalDataModel GetExternalDataById(int externalDataId);
        Task<int> GetOrCreateExternalDataId(ExternalDataModel model);
        List<ScannedDataModel> GetFinishedListForHtml(List<ScannedDataBaseModel> stocksList);
        Task<IEnumerable<AutocompleteModel>> ExternalAutoComplete();
        bool CheckIfExists(string barcode);
        Task UpdateExternalDataAsync(ExternalDataModel item);
        Task UpdateExternalDataAsyncByBarcode(ExternalDataModel item);
    }
}
