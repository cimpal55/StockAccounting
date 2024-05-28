using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Repositories;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Web.Services.Interfaces;
using StockAccounting.Web.Utils;

namespace StockAccounting.Web.Services
{
    public class PaginationService : IPaginationService
    {
        private readonly IExternalDataRepository _externalRepository;
        private readonly IScannedDataRepository _scannedRepository;
        private readonly IInventoryDataRepository _inventoryRepository;
        private readonly IEmployeeDataRepository _employeeRepository;
        private readonly IStockDataRepository _stockRepository;
        private readonly IToolkitRepository _toolkitRepository;

        public PaginationService(
            IExternalDataRepository externalDataRepository,
            IScannedDataRepository scannedRepository,
            IInventoryDataRepository inventoryDataRepository,
            IEmployeeDataRepository employeeRepository,
            IStockDataRepository stockRepository,
            IToolkitRepository toolkitRepository)
        {
            _externalRepository = externalDataRepository;
            _scannedRepository = scannedRepository;
            _inventoryRepository = inventoryDataRepository;
            _employeeRepository = employeeRepository;
            _stockRepository = stockRepository;
            _toolkitRepository = toolkitRepository;
        }

        public async Task<PaginatedData<ExternalDataModel>> PaginatedProducts(int pageIndex, int pageSize)
        {
            var products = await _externalRepository.GetExternalDataAsync();
            return PaginatedData<ExternalDataModel>.CreateList(products, pageIndex, pageSize);
        }

        public async Task<PaginatedData<ExternalDataModel>> PaginatedSearchedProducts(int pageIndex, int pageSize, string searchText)
        {
            var products = await _externalRepository.GetExternalDataSearchTextAsync(searchText);
            return PaginatedData<ExternalDataModel>.CreateList(products, pageIndex, pageSize);
        }

        public async Task<PaginatedData<ScannedDataModel>> PaginatedScannedData(int pageIndex, int pageSize, int docId)
        {
            var details = await _scannedRepository.GetScannedDataByDocumentIdAsync(docId);
            return PaginatedData<ScannedDataModel>.CreateList(details, pageIndex, pageSize);
        }

        public async Task<PaginatedData<InventoryDataModel>> PaginatedDocuments(int pageIndex, int pageSize)
        {
            var docs = await _inventoryRepository.GetInventoryDataAsync();
            return PaginatedData<InventoryDataModel>.CreateList(docs, pageIndex, pageSize);
        }

        public async Task<PaginatedData<InventoryDataModel>> PaginatedSearchedDocuments(int pageIndex, int pageSize, string searchText)
        {
            var docs = await _inventoryRepository.GetInventoryDataSearchTextAsync(searchText);
            return PaginatedData<InventoryDataModel>.CreateList(docs, pageIndex, pageSize);
        }

        public async Task<PaginatedData<EmployeeDataModel>> PaginatedEmployees(int pageIndex, int pageSize)
        {
            var employees = await _employeeRepository.GetEmployeesAsync();
            return PaginatedData<EmployeeDataModel>.CreateList(employees, pageIndex, pageSize);
        }

        public async Task<PaginatedData<EmployeeDataModel>> PaginatedSearchedEmployees(int pageIndex, int pageSize, string searchText)
        {
            var employees = await _employeeRepository.GetEmployeesSearchTextAsync(searchText);
            return PaginatedData<EmployeeDataModel>.CreateList(employees, pageIndex, pageSize);
        }
        
        public async Task<PaginatedData<StockDataModel>> PaginatedEmployeeDetails(int pageIndex, int pageSize, int employeeId)
        {
            var details = await _employeeRepository.GetEmployeeDetailsByIdAsync(employeeId);
            return PaginatedData<StockDataModel>.CreateList(details, pageIndex, pageSize);
        }

        public async Task<PaginatedData<StockEmployeesModel>> PaginatedStockDetails(int pageIndex, int pageSize, int stockId)
        {
            var stocks = await _stockRepository.GetStockDetailsByIdAsync(stockId);
            return PaginatedData<StockEmployeesModel>.CreateList(stocks, pageIndex, pageSize);
        }

        public async Task<PaginatedData<ToolkitModel>> PaginatedToolkits(int pageIndex, int pageSize)
        {
            var toolkits = await _toolkitRepository.GetToolkitDataAsync();
            return PaginatedData<ToolkitModel>.CreateList(toolkits, pageIndex, pageSize);
        }

        public async Task<PaginatedData<ToolkitModel>> PaginatedSearchedToolkits(int pageIndex, int pageSize, string searchText)
        {
            var toolkits = await _toolkitRepository.GetToolkitDataBySearchTextAsync(searchText);
            return PaginatedData<ToolkitModel>.CreateList(toolkits, pageIndex, pageSize);
        }
    }
}
