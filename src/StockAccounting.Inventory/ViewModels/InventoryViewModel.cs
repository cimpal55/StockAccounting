using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Sextant;
using Splat;
using StockAccounting.Inventory.Models;
using StockAccounting.Inventory.Repositories.Interfaces;
using StockAccounting.Inventory.Services;
using StockAccounting.Inventory.Services.Interfaces;
using StockAccounting.Inventory.ViewModels.Base;

namespace StockAccounting.Inventory.ViewModels
{
    public partial class InventoryDataViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IInventoryDataRepository _inventoryDataRepository;
        private readonly SourceCache<InventoryListItem, int> _itemsSource = new(static x => x.Key);
        private readonly SourceCache<InventoryListItem, int> _searchedItemsSource = new(static x => x.Key);
        private static readonly string _viewTitle = "Documents";

        public InventoryDataViewModel(
            IInventoryDataRepository inventoryDataRepository,
            INavigationService navigationService)
            : base(_viewTitle)
        {
            _inventoryDataRepository = inventoryDataRepository;
            _navigationService = navigationService;

            _itemsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(Items)
                .Subscribe()
                .DisposeWith(D);

            _searchedItemsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(SearchedItems)
                .Subscribe()
                .DisposeWith(D);

            SearchBarCommand = CreateSearchBarCommand();
        }

        [ObservableProperty] public InventoryListItem _selectedDocument;

        [ObservableProperty] public string _searchedText;

        public ObservableCollectionExtended<InventoryListItem> Items { get; } = new();

        public ObservableCollectionExtended<InventoryListItem> SearchedItems { get; set; } = new();

        public ReactiveCommand<string, Unit> SearchBarCommand { get; }

        public override async Task LoadAsync()
        {
            var items = await _inventoryDataRepository.GetFullListAsync()
                .ConfigureAwait(false);

            var listItems = items
                .Select(static x => new InventoryListItem(x.Id)
                {
                    ExternalId = x.ExternalId,
                    Name = x.Name,
                    Employee1 = x.Employee1CheckerId.ToString(),
                    Employee2 = x.Employee2CheckerId.ToString(),
                    Status = x.Status.ToString(),
                    Created = x.Created
                })
                .OrderBy(x => x.Created);
            
            _itemsSource.Clear();
            _itemsSource.AddOrUpdate(listItems);
        }

        [RelayCommand]
        private async Task ShowDetails() =>
            await _navigationService.GoToScannedData(SelectedDocument.ExternalId);

        private ReactiveCommand<string, Unit> CreateSearchBarCommand()
        {
            return ReactiveCommand
                .CreateFromTask<string>(async z =>
                {
                    var items = await _inventoryDataRepository.GetListByNameAsync(SearchedText)
                        .ConfigureAwait(false);

                    var listItems = items
                        .Select(static x => new InventoryListItem(x.Id)
                        {
                            ExternalId = x.ExternalId,
                            Name = x.Name,
                            Employee1 = x.Employee1CheckerId.ToString(),
                            Employee2 = x.Employee2CheckerId.ToString(),
                            Created = x.Created,
                        });

                    _itemsSource.Clear();
                    _itemsSource.AddOrUpdate(listItems);
                    //}, canExecute);
                });
        }
    }
}
