using StockAccounting.Core.Data.Models.Data.Toolkit;
using StockAccounting.Core.Data.Models.Data.ToolkitExternal;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IToolkitRepository
    {
        IQueryable<ToolkitModel> GetToolkitDataQueryable();

        IQueryable<ToolkitModel> GetToolkitDataBySearchTextQueryable(string searchText);

        Task<IEnumerable<ToolkitExternalModel>> GetToolkitExternalDataAsync();

        Task<int> InsertToolkitWithIdentityAsync(ToolkitModel model);

        Task InsertExternalToolkit(ToolkitExternalModel model);

        Task<string> ReturnToolkitBarcode();

    }
}
