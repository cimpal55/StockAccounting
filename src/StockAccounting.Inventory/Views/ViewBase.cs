using StockAccounting.Inventory.ViewModels.Base;

namespace StockAccounting.Inventory.Views
{
    public class ViewBase : ContentPage
    {
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is not IViewModelBase ivmb)
            {
                return;
            }

            await ivmb.InitializeAsyncCommand.ExecuteAsync(null);
        }
    }
}
