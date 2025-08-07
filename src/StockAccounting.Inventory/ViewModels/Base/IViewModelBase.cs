using CommunityToolkit.Mvvm.Input;

namespace StockAccounting.Inventory.ViewModels.Base
{
    public interface IViewModelBase
    {
        IAsyncRelayCommand InitializeAsyncCommand { get; }
    }
}
