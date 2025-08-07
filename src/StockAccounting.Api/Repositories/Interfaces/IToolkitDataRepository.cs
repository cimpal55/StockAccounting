using StockAccounting.Core.Data.Models.Data.Toolkit;
using StockAccounting.Core.Data.Models.Data.ToolkitExternal;

namespace StockAccounting.Api.Repositories.Interfaces
{
    public interface IToolkitDataRepository
    {
        Task<List<ToolkitModel>> GetToolkitData();

        Task<List<ToolkitExternalModel>> GetToolkitExternalDataByToolkitIdAsync(int id);
    }
}
