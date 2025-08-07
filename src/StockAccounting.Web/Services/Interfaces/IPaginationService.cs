using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.Data.EmployeeData;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.Data.StockData;
using StockAccounting.Core.Data.Models.Data.StockEmployees;
using StockAccounting.Core.Data.Models.Data.Toolkit;
using StockAccounting.Core.Data.Models.DataTransferObjects;

namespace StockAccounting.Web.Services.Interfaces
{
    public interface IPaginationService
    {
        // External Data
        Task<PaginatedData<ExternalDataModel>> PaginatedProducts(int pageIndex, int pageSize);
        Task<PaginatedData<ExternalDataModel>> PaginatedSearchedProducts(int pageIndex, int pageSize, string searchText);

        // Scanned Data
        Task<PaginatedData<ScannedDataModel>> PaginatedScannedData(int pageIndex, int pageSize, int docId);

        // Documents
        Task<PaginatedData<DocumentDataModel>> PaginatedDocuments(int pageIndex, int pageSize);
        Task<PaginatedData<DocumentDataModel>> PaginatedSearchedDocuments(int pageIndex, int pageSize, string searchText);

        Task<PaginatedData<DocumentDataModel>> PaginatedDocumentsSorted(int pageIndex, int pageSize, string searchText,
            string sortColumn, string sortDirection);

        // Inventory
        Task<PaginatedData<InventoryListModel>> PaginatedInventory(int pageIndex, int pageSize);
        Task<PaginatedData<InventoryListModel>> PaginatedSearchedInventory(int pageIndex, int pageSize, string searchText);

        // InventoryDetails
        Task<PaginatedData<InventoryDetailsListModel>> PaginatedInventoryDetails(int pageIndex, int pageSize, int inventoryId);
        Task<PaginatedData<InventoryDetailsListModel>> PaginatedSearchedInventoryDetails(int pageIndex, int pageSize, string searchText, int inventoryId);

        // Employees
        Task<PaginatedData<EmployeeDataModel>> PaginatedEmployees(int pageIndex, int pageSize);
        Task<PaginatedData<EmployeeDataModel>> PaginatedSearchedEmployees(int pageIndex, int pageSize, string searchText);
        Task<PaginatedData<EmployeeDetailsListModel>> PaginatedEmployeeDetails(int pageIndex, int pageSize,
            int employeeId);
        Task<PaginatedData<EmployeeDetailLeftQuantityListModel>> PaginatedEmployeeDetailLeftQuantity(int pageIndex, int pageSize,
            int employeeId,
            int externalDataId);

        // Stocks
        Task<PaginatedData<StockDataModel>> PaginatedStocks(int pageIndex, int pageSize);
        Task<PaginatedData<StockDataModel>> PaginatedSearchedStocks(int pageIndex, int pageSize, string searchText);
        Task<PaginatedData<StockEmployeesModel>> PaginatedStockDetails(int pageIndex, int pageSize, int stockId);

        // Toolkits
        Task<PaginatedData<ToolkitModel>> PaginatedToolkits(int pageIndex, int pageSize);
        Task<PaginatedData<ToolkitModel>> PaginatedSearchedToolkits(int pageIndex, int pageSize, string searchText);
    }
}
