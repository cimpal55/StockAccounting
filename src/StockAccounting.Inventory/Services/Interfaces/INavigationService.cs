namespace StockAccounting.Inventory.Services.Interfaces
{
    public interface INavigationService
    {
        Task GoToMainWindow();

        Task GoToEmployee();

        Task GoToInventoryData();

        Task GoToScannedData(int id);

        Task GoToScannedDataAdd(int id);
    }
}
