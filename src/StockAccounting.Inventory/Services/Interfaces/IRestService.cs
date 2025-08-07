using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Inventory.Models;
using System.Collections.ObjectModel;
using StockAccounting.Core.Android.Models.Data;

namespace StockAccounting.Inventory.Services.Interfaces
{
    public interface IRestService
    {
        Task<bool> CheckConnectionAsync(string ip);

        Task<string> SendScannedDataUpdateAsync(string jsonData, int docId);

        Task<string> SendScannedDataInsertAsync(string jsonData, int docId);

        Task<string> SendInventoryDataAsync(string jsonData);

        Task<int> InsertInventoryDataAsync(string jsonData);

        Task<int> GetInventoryDataIdByName(string docName);

        Task<IReadOnlyList<InventoryDataJson>?> GetInventoryDataAsync();

        Task<IReadOnlyList<InventoryDataJson>?> GetLatestInventoryDataAsync(string jsonData);

        Task<IReadOnlyList<InventoryDataJson>?> GetCheckedInventoryDataAsync();

        Task<IReadOnlyList<ScannedInventoryDataJson>?> GetLatestScannedDataAsync(string jsonData);

        Task<IReadOnlyList<ScannedInventoryDataJson>?> GetScannedDataAsync();

        Task<IReadOnlyList<ScannedInventoryDataJson>> GetDetListByDocIdAsync(int docId);

        Task<IReadOnlyList<EmployeeJson>?> GetEmployeesAsync();

        Task<IReadOnlyList<InventoryDataJson>?> GetInprocessInventoryDataAsync();

        Task<int> CreateDocumentAfterInventoryCheck(string jsonData);

        Task CreateDetailsAfterInventoryCheck(string jsonData, int docId);

        Task<ExternalDataRecord> GetExternalDataIdByBarcode(string barcode);

        Task<ExternalDataModel> GetExternalDataById(int id);

        Task<ObservableCollection<ExternalDataModel>> GetExternalData();

        Task<IReadOnlyList<ScannedInventoryDataJson>> GetDetailsByEmployeeId(int id);
    }
}