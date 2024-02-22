using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IScannedDataRepository
    {
        //Task<int> InsertScannedDataAsync(ScannedDataBaseModel item);
        Task UpdateScannedDataAsync(ScannedDataBaseModel item);
        //Task DeleteScannedDataAsync(ScannedDataBaseModel item);
        Task<IEnumerable<SynchronizationModel>> GetBarcodesAsync();
        Task SynchronizationWithServiceTrader(IEnumerable<SynchronizationModel> barcodes);
        Task CreateDocumentForSynchronization(SynchronizationModel item);
        Task<IEnumerable<ScannedDataModel>> GetScannedDataByIdAsync(int inventoryDataId);
        Task<IEnumerable<ScannedReportExcelModel>> GetScannedReportDataAsync(IEnumerable<int> employeeList, FileExport mode);
        string FileExportMode(FileExport mode);
        bool IsSynchronizationDocument(int inventoryDataId);
        int ReturnDocumentEmployees(int inventoryDataId);
        string ReturnEmployeeNotificationEmail(int employeeId);
    }
}
