using StockAccounting.Inventory.Services.Interfaces;

namespace StockAccounting.Inventory.Services
{
    public class NavigationService : INavigationService
    {
        public Task GoToMainWindow()
        {
            Shell.Current.Navigation.PopToRootAsync();
            return Shell.Current.GoToAsync("//MainView");
        }

        public Task GoToEmployee() =>
            Shell.Current.GoToAsync("EmployeeView");

        public Task GoToInventoryData() =>
            Shell.Current.GoToAsync("InventoryView");

        public async Task GoToScannedData(int id)
        {
            var parameters = new Dictionary<string, object> { { "InventoryDataId", id } };
            await Shell.Current.GoToAsync("ScannedView", parameters);
        }

        public async Task GoToScannedDataAdd(int id)
        {
            var parameters = new Dictionary<string, object> { { "InventoryDataId", id } };
            await Shell.Current.GoToAsync("ScannedAddView", parameters);
        }
    }
}
