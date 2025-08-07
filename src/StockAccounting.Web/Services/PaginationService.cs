using System.Linq.Expressions;
using LinqToDB;
using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.Data.EmployeeData;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.Data.StockData;
using StockAccounting.Core.Data.Models.Data.StockEmployees;
using StockAccounting.Core.Data.Models.Data.Toolkit;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Web.Repositories;
using StockAccounting.Web.Repositories.Interfaces;
using StockAccounting.Web.Services.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Web.Services
{
    public class PaginationService : IPaginationService
    {
        private readonly IDataRepository _repository;

        public PaginationService(IDataRepository repository)
        {
            _repository = repository;
        }

        private async Task<PaginatedData<T>> PaginateAsync<T>(
            Func<IQueryable<T>> getData, int pageIndex, int pageSize)
        {
            var query = getData();

            return await PaginatedData<T>.CreateListAsync(query, pageIndex, pageSize);
        }

        private static readonly Dictionary<string, Expression<Func<DocumentDataModel, object>>> SortColumns = new()
        {
            { "Created", x => x.Created },
            { "Employee1", x => x.Employee1 },
            { "Employee2", x => x.Employee2 },
        };

        private IQueryable<T> ApplySort<T>(IQueryable<T> query, string sortColumn, string sortDirection)
        {
            if (!SortColumns.ContainsKey(sortColumn))
            {
                sortColumn = "Created";
            }

            var sortLambda = SortColumns[sortColumn];

            string methodName = sortDirection.ToLower() == "asc" ? "OrderBy" : "OrderByDescending";

            var orderByMethod = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(typeof(T), sortLambda.ReturnType);

            return (IQueryable<T>)orderByMethod.Invoke(null, new object[] { query, sortLambda });
        }


        // External Data
        public async Task<PaginatedData<ExternalDataModel>> PaginatedProducts(int pageIndex, int pageSize) =>
            await PaginateAsync<ExternalDataModel>(() =>
                _repository.ExternalData.GetExternalDataQueryable(), pageIndex, pageSize);

        public async Task<PaginatedData<ExternalDataModel>> PaginatedSearchedProducts(int pageIndex, int pageSize, string searchText) =>
            await PaginateAsync<ExternalDataModel>(() =>
                _repository.ExternalData.GetExternalDataSearchTextQueryable(searchText), pageIndex, pageSize);

        // Scanned Data
        public async Task<PaginatedData<ScannedDataModel>> PaginatedScannedData(int pageIndex, int pageSize, int docId) =>
            await PaginateAsync(() => _repository.ScannedData.GetScannedDataByDocumentIdQueryable(docId), pageIndex, pageSize);

        // Documents
        public async Task<PaginatedData<DocumentDataModel>> PaginatedDocuments(int pageIndex, int pageSize) =>
            await PaginateAsync(() => _repository.Documents.GetDocumentDataQueryable(), pageIndex, pageSize);

        public async Task<PaginatedData<DocumentDataModel>> PaginatedSearchedDocuments(int pageIndex, int pageSize, string searchText) =>
            await PaginateAsync(() => _repository.Documents.GetDocumentDataSearchTextQueryable(searchText), pageIndex, pageSize);

        public async Task<PaginatedData<DocumentDataModel>> PaginatedDocumentsSorted(
            int pageIndex, int pageSize, string searchText, string sortColumn, string sortDirection)
        {
            var query = _repository.Documents.GetDocumentDataQueryable();

            if (!string.IsNullOrEmpty(searchText) && searchText != "dummyText")
            {
                query = query.Where(d => d.Employee1.Contains(searchText) || d.Employee2.Contains(searchText));
            }

            query = ApplySort(query, sortColumn, sortDirection);

            return await PaginatedData<DocumentDataModel>.CreateListAsync(query, pageIndex, pageSize);
        }

        // Inventory
        public async Task<PaginatedData<InventoryListModel>> PaginatedInventory(int pageIndex, int pageSize) =>
            await PaginateAsync(() => _repository.Inventory.GetInventoryDataQueryable(), pageIndex, pageSize);

        public async Task<PaginatedData<InventoryListModel>> PaginatedSearchedInventory(int pageIndex, int pageSize,
            string searchText) =>
            await PaginateAsync(() => _repository.Inventory.GetInventoryDataSearchTextQueryable(searchText), pageIndex,
                pageSize);

        // Inventory Details
        public async Task<PaginatedData<InventoryDetailsListModel>> PaginatedInventoryDetails(int pageIndex, int pageSize, int inventoryId) =>
            await PaginateAsync(() => _repository.Inventory.GetInventoryDetailsQueryable(inventoryId), pageIndex, pageSize);

        public async Task<PaginatedData<InventoryDetailsListModel>> PaginatedSearchedInventoryDetails(int pageIndex, int pageSize,
            string searchText, int inventoryId) =>
            await PaginateAsync(() => _repository.Inventory.GetInventoryDetailsQueryable(inventoryId, searchText), pageIndex,
                pageSize);

        // Employees
        public async Task<PaginatedData<EmployeeDataModel>> PaginatedEmployees(int pageIndex, int pageSize) =>
            await PaginateAsync(() => _repository.Employees.GetEmployeesQueryable(), pageIndex, pageSize);

        public async Task<PaginatedData<EmployeeDataModel>> PaginatedSearchedEmployees(int pageIndex, int pageSize, string searchText) =>
            await PaginateAsync(() => _repository.Employees.GetEmployeesSearchTextQueryable(searchText), pageIndex, pageSize);

        public async Task<PaginatedData<EmployeeDetailsListModel>> PaginatedEmployeeDetails(int pageIndex, int pageSize, int employeeId) =>
            await PaginateAsync(() => _repository.Employees.GetEmployeeDetailsByIdQueryable(employeeId), pageIndex, pageSize);

        public async Task<PaginatedData<EmployeeDetailLeftQuantityListModel>> PaginatedEmployeeDetailLeftQuantity(int pageIndex, int pageSize, int employeeId,
            int externalDataId) =>
            await PaginateAsync(() => _repository.Employees.GetEmployeeDetailLeftQuantityByIdQueryable(employeeId, externalDataId),
                pageIndex, pageSize);

        // Stocks
        public async Task<PaginatedData<StockDataModel>> PaginatedStocks(int pageIndex, int pageSize) =>
            await PaginateAsync(() => _repository.Stocks.GetStockDataQueryable(), pageIndex, pageSize);

        public async Task<PaginatedData<StockDataModel>> PaginatedSearchedStocks(int pageIndex, int pageSize, string searchText) =>
            await PaginateAsync(() => _repository.Stocks.GetStockDataSearchTextQueryable(searchText), pageIndex, pageSize);

        public async Task<PaginatedData<StockEmployeesModel>> PaginatedStockDetails(int pageIndex, int pageSize, int stockId) =>
            await PaginateAsync(() => _repository.Stocks.GetStockDetailsByIdQueryable(stockId), pageIndex, pageSize);

        // Toolkits
        public async Task<PaginatedData<ToolkitModel>> PaginatedToolkits(int pageIndex, int pageSize) =>
            await PaginateAsync(() => _repository.Toolkits.GetToolkitDataQueryable(), pageIndex, pageSize);

        public async Task<PaginatedData<ToolkitModel>> PaginatedSearchedToolkits(int pageIndex, int pageSize, string searchText) =>
            await PaginateAsync(() => _repository.Toolkits.GetToolkitDataBySearchTextQueryable(searchText), pageIndex, pageSize);

    }
}
