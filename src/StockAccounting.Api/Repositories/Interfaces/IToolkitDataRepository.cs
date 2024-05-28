using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Api.Repositories.Interfaces
{
    public interface IToolkitDataRepository
    {
        Task<List<ToolkitModel>> GetToolkitData();

        Task<List<ToolkitExternalModel>> GetToolkitExternalDataByToolkitIdAsync(int id);
    }
}
