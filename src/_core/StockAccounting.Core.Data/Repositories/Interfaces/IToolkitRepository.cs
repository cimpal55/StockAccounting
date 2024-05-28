using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IToolkitRepository
    {
        Task<IEnumerable<ToolkitModel>> GetToolkitDataAsync();

        Task<IEnumerable<ToolkitModel>> GetToolkitDataBySearchTextAsync(string searchText); 

        Task<IEnumerable<ToolkitExternalModel>> GetToolkitExternalDataAsync();

        Task<int> InsertToolkitWithIdentityAsync(ToolkitModel model);

        Task InsertExternalToolkit(ToolkitExternalModel model);

        Task<string> ReturnToolkitBarcode();

    }
}
