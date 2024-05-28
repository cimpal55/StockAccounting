using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IScannedDataRepository
    {
        Task<ScannedDataBaseModel> ReturnScannedDataById(int scannedDataId);
        Task UpdateScannedDataAsync(ScannedDataBaseModel item);
        Task<IEnumerable<SynchronizationModel>> GetBarcodesAsync();
        Task SynchronizationWithServiceTrader(IEnumerable<SynchronizationModel> barcodes);
        Task<IEnumerable<ScannedDataModel>> GetScannedDataByDocumentIdAsync(int inventoryDataId);
        Task<IEnumerable<ScannedReportExcelModel>> GetScannedReportDataAsync(IEnumerable<int> employeeList, FileExport mode);
        string FileExportMode(FileExport mode);
        bool IsSynchronizationDocument(int inventoryDataId);
        int ReturnDocumentEmployees(int inventoryDataId);
        string ReturnEmployeeNotificationEmail(int employeeId);
        Task<Dictionary<string, string>> GetDocumentNumber(int employeeId, int inventoryDataId);
        Task<StockEmployeesBaseModel> GetStockEmployeeByScannedData(ScannedDataBaseModel model);
    }
}
