using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Web.Repositories.Interfaces;

namespace StockAccounting.Web.Repositories
{
    public class DataRepository : IDataRepository
    {
        public IExternalDataRepository ExternalData { get; }
        public IScannedDataRepository ScannedData { get; }
        public IDocumentDataRepository Documents { get; }
        public IInventoryRepository Inventory { get; }
        public IEmployeeDataRepository Employees { get; }
        public IStockDataRepository Stocks { get; }
        public IToolkitRepository Toolkits { get; }

        public DataRepository(
            IExternalDataRepository externalData,
            IScannedDataRepository scannedData,
            IDocumentDataRepository documents,
            IInventoryRepository inventory,
            IEmployeeDataRepository employees,
            IStockDataRepository stocks,
            IToolkitRepository toolkits)
        {
            ExternalData = externalData;
            ScannedData = scannedData;
            Documents = documents;
            Inventory = inventory;
            Employees = employees;
            Stocks = stocks;
            Toolkits = toolkits;
            Inventory = inventory;
        }
    }

}
