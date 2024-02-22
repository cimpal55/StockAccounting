using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.Data.Excel;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IStockDataRepository
    {
        Task<IEnumerable<StockDataModel>> GetStockDataAsync();
        Task<IEnumerable<StockEmployeesModel>> GetStockDetailsByIdAsync(int id);
        Task<StockDataBaseModel> CheckIfStockExists(int externalDataId);
        Task InsertStockData(StockEmployeesModel stockEmployeeData);
        string FileExportMode(FileExport mode);
        Task<IEnumerable<StockReportExcelModel>> GetStockReportDataAsync(IEnumerable<int> stocksList, FileExport mode);

    }
}
