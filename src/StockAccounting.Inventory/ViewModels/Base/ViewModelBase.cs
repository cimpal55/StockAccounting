using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sextant;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace StockAccounting.Inventory.ViewModels.Base
{
    public partial class ViewModelBase : ObservableObject, IViewModelBase
    {
        [ObservableProperty]
        private bool _isLoading;

        protected readonly CompositeDisposable D = new();

        public IAsyncRelayCommand InitializeAsyncCommand { get; }

        public string? Parameter;

        public ViewModelBase(string viewTitle)
        {
            viewTitle = viewTitle;

            InitializeAsyncCommand = new AsyncRelayCommand(
                async () =>
                {
                    IsLoading = true;
                    await Loading(LoadAsync);
                    IsLoading = false;
                });
        }

        public string ViewTitle { get; set; }

        public string Id => ViewTitle;

        protected async Task Loading(Func<Task> unitOfWork)
        {
            await unitOfWork();
        }

        public virtual Task LoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}
