using StockAccounting.Checklist.Models.Data;
using StockAccounting.Checklist.Models.Data.DataTransferObjects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StockAccounting.Checklist.Services.Interfaces
{
    public interface IInventoryDataService
    {
        Task<ObservableCollection<InventoryDataModel>> GetInventoryDataAsync();
        Task<ScannedModel> InsertInventoryData(ScannedModel data);
    }
}
