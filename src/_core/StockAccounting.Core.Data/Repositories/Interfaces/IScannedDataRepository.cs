using StockAccounting.Core.Data.Enums;
using StockAccounting.Core.Data.Models.Data.Excel;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.Data.StockEmployees;
using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IScannedDataRepository
    {
        Task<ScannedDataBaseModel> ReturnScannedDataById(int scannedDataId);
        Task UpdateScannedDataAsync(ScannedDataBaseModel item);
        Task<IEnumerable<SynchronizationModel>> GetBarcodesAsync();
        Task SynchronizationWithServiceTrader(IEnumerable<SynchronizationModel> barcodes);
        IQueryable<ScannedDataModel> GetScannedDataByDocumentIdQueryable(int documentDataId);
        Task<IEnumerable<ScannedReportExcelModel>> GetScannedReportDataAsync(IEnumerable<int> employeeList, FileExport mode);
        string FileExportMode(FileExport mode);
        bool IsSynchronizationDocument(int documentDataId);
        int ReturnDocumentEmployees(int documentDataId);
        string ReturnEmployeeNotificationEmail(int employeeId);
        Task<Dictionary<string, string>> GetDocumentNumber(int employeeId, int documentDataId);
        Task<StockEmployeesBaseModel> GetStockEmployeeByScannedData(ScannedDataBaseModel model);
    }
}
