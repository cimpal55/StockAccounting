using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using System.Collections.ObjectModel;

namespace StockAccounting.Api.Repositories.Interfaces
{
    public interface IInventoryDataRepository
    {
        Task<List<InventoryDataModel>> GetInventoryData();
        Task<int?> InsertInventoryDataAsync(ScannedModel data);

    }
}
