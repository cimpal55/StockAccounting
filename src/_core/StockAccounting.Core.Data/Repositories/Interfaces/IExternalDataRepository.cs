using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IExternalDataRepository
    {
        Task<IEnumerable<ExternalDataModel>> GetExternalDataAsync();
        Task<IEnumerable<ExternalDataModel>> GetExternalDataSearchTextAsync(string searchText);
        ExternalDataModel GetExternalDataById(int externalDataId);
        Task<int> GetOrCreateExternalDataId(ExternalDataModel model);
        List<ScannedDataModel> GetFinishedListForHtml(List<ScannedDataBaseModel> stocksList);
        Task<IEnumerable<AutocompleteModel>> ExternalAutoComplete();
        Task<bool> CheckIfExists(string barcode);
        Task UpdateExternalDataAsync(ExternalDataModel item);
        Task UpdateExternalDataAsyncByBarcode(ExternalDataModel item);
    }
}
