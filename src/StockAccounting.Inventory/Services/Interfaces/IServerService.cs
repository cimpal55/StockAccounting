using StockAccounting.Inventory.Models;

namespace StockAccounting.Inventory.Services.Interfaces
{
    public interface IServerService
    {
        Task<IReadOnlyList<ScannedInventoryDataJson>?> GetScannedDataFromServer();

        Task<IReadOnlyList<InventoryDataJson>?> GetInventoryDataFromServer();

        Task<IReadOnlyList<InventoryDataJson>?> GetLatestInventoryDataFromServer();

        Task<IReadOnlyList<EmployeeJson>?> GetEmployeesFromServer();

        Task GetLatestScannedDataFromServer(IReadOnlyList<InventoryDataJson> docs);
    }
}
