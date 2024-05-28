using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.Data.Excel;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IStockDataRepository
    {
        Task<IEnumerable<StockDataModel>> GetStockDataAsync();
        Task<IEnumerable<StockEmployeesModel>> GetStockDetailsByIdAsync(int stockDataId);
        Task<StockDataBaseModel> CheckIfStockExists(int externalDataId);
        Task<bool> CheckIfStockEmployeeExists();
        Task<List<string>> ReturnStockEmployeesCodes();
        Task InsertStockData(StockEmployeesModel stockEmployeeData);
        string FileExportMode(FileExport mode);
        Task<IEnumerable<StockReportExcelModel>> GetStockReportDataAsync(IEnumerable<int> stocksList, FileExport mode);
        Task UpdateStockQuantity(int stockDataId, decimal quantity);
        Task<IEnumerable<StockEmployeesModel>> GetStockLeftQuantity(int stockDataId); 
    }
}
