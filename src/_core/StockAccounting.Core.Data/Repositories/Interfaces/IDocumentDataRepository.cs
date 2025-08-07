using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.Data.StockEmployees;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IDocumentDataRepository
    {
        IQueryable<DocumentDataModel> GetDocumentDataQueryable();
        Task<DocumentDataModel> GetDocumentDataByIdAsync(int id);
        IQueryable<DocumentDataModel> GetDocumentDataSearchTextQueryable(string searchText);
        Task UpdateDocumentDataAsync(DocumentDataBaseModel item);
        Task<int> InsertWithIdentityAsync(DocumentDataBaseModel item);
        bool CheckIfDocumentHasScannedData(int documentId);
        Task<int> ReturnDocumentIdIfExists(DocumentDataBaseModel item);
        Task<List<ScannedDataBaseModel>> ReturnDocumentScannedData(int documentId);
        Task<List<StockEmployeesBaseModel>> ReturnStockEmployeesByDocumentNumber(ScannedDataBaseModel data);
        Task<List<StockEmployeesBaseModel>> ReturnStockEmployeesBySerialNumber(string serialNumber);
        Task<bool> CheckDocumentSynchronizationAsync(int employeeId);
        Task<bool> ReturnDocumentSynchronizationAsync(int documentId);
    }
}
