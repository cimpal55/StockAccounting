using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.Web.Repositories.Interfaces
{
    public interface IDataRepository
    {
        IExternalDataRepository ExternalData { get; }
        IScannedDataRepository ScannedData { get; }
        IDocumentDataRepository Documents { get; }
        IInventoryRepository Inventory { get; }
        IEmployeeDataRepository Employees { get; }
        IStockDataRepository Stocks { get; }
        IToolkitRepository Toolkits { get; }
    }
}
