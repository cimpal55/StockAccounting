using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Web.Utils;

namespace StockAccounting.Web.Services.Interfaces
{
    public interface IPaginationService
    {
        Task<PaginatedData<ExternalDataModel>> PaginatedProducts(int pageIndex, int pageSize);

        Task<PaginatedData<ExternalDataModel>> PaginatedSearchedProducts(int pageIndex, int pageSize, string searchText);

        Task<PaginatedData<ScannedDataModel>> PaginatedScannedData(int pageIndex, int pageSize, int docId);

        Task<PaginatedData<InventoryDataModel>> PaginatedDocuments(int pageIndex, int pageSize);

        Task<PaginatedData<InventoryDataModel>> PaginatedSearchedDocuments(int pageIndex, int pageSize, string searchText);

        Task<PaginatedData<EmployeeDataModel>> PaginatedEmployees(int pageIndex, int pageSize);

        Task<PaginatedData<StockDataModel>> PaginatedEmployeeDetails(int pageIndex, int pageSize, int employeeId);

        Task<PaginatedData<EmployeeDataModel>> PaginatedSearchedEmployees(int pageIndex, int pageSize, string searchText);

        Task<PaginatedData<StockEmployeesModel>> PaginatedStockDetails(int pageIndex, int pageSize, int stockId);

        Task<PaginatedData<ToolkitModel>> PaginatedToolkits(int pageIndex, int pageSize);

        Task<PaginatedData<ToolkitModel>> PaginatedSearchedToolkits(int pageIndex, int pageSize, string searchText);
    }
}
