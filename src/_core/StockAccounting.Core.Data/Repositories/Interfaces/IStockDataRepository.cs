using StockAccounting.Core.Data.Enums;
using StockAccounting.Core.Data.Models.Data.Excel;
using StockAccounting.Core.Data.Models.Data.StockData;
using StockAccounting.Core.Data.Models.Data.StockEmployees;

namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IStockDataRepository
    {
        IQueryable<StockDataModel> GetStockDataQueryable();
        IQueryable<StockDataModel> GetStockDataSearchTextQueryable(string searchText);
        IQueryable<StockEmployeesModel> GetStockDetailsByIdQueryable(int stockDataId);
        Task<IEnumerable<StockEmployeesModel>> GetStockDetailsByIdAsync(int stockDataId);
        Task<StockDataBaseModel> CheckIfStockExists(int externalDataId);
        Task<bool> CheckIfStockEmployeeExists();
        Task<List<string>> ReturnStockEmployeesCodes();
        Task InsertStockData(StockEmployeesModel stockEmployeeData);
        Task EditStockData(StockEmployeesModel stockEmployeeData, decimal quantityBefore);
        Task InsertStockDataAfterInventory(StockEmployeesModel stockEmployeeData);
        string FileExportMode(FileExport mode);
        Task<IEnumerable<StockReportExcelModel>> GetStockReportDataAsync(IEnumerable<int> stocksList, FileExport mode);
        Task UpdateStockQuantity(int stockDataId, decimal quantity);
        Task<IEnumerable<StockEmployeesModel>> GetStockLeftQuantity(int stockDataId);
    }
}
