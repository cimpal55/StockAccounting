using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using Acr.UserDialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Sextant;
using Splat;
using StockAccounting.Core.Android.Data;
using StockAccounting.Core.Android.Enums;
using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Core.Android.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.Inventory.Models;
using StockAccounting.Inventory.Repositories.Interfaces;
using StockAccounting.Inventory.Services;
using StockAccounting.Inventory.Services.Interfaces;
using StockAccounting.Inventory.ViewModels.Base;

namespace StockAccounting.Inventory.ViewModels;

public partial class ScannedInventoryDataViewModel : ViewModelBase, IQueryAttributable
{
    private readonly INavigationService _navigationService;
    private readonly IRestService _restService;
    private readonly IServerService _serverService;
    private readonly DatabaseContext _context;

    private readonly IScannedInventoryDataRepository _scannedInventoryDataRepository;
    private readonly IInventoryDataRepository _inventoryDataRepository;
    private readonly IAdministrationRepository _administrationRepository;

    private static readonly string ApiIp = Preferences.Get("ApiIp", "default_value");
    private static readonly string SyncPass = Preferences.Get("SyncPass", "default_value");
    private static string _viewTitle = "Details";

    private readonly SourceCache<ScannedInventoryListItem, int> _itemsSource = new(static x => x.Key);
    private readonly SourceCache<InventoryListItem, int> _searchedItemsSource = new(static x => x.Key);

    public ScannedInventoryDataViewModel(
        IScannedInventoryDataRepository scannedDataRepository,
        IInventoryDataRepository inventoryDataRepository,
        IAdministrationRepository administrationRepository,
        IRestService restService,
        INavigationService navigationService,
        DatabaseContext context, 
        IServerService serverService)
            : base(_viewTitle)
    {
        _scannedInventoryDataRepository = scannedDataRepository;
        _inventoryDataRepository = inventoryDataRepository;
        _administrationRepository = administrationRepository;

        _restService = restService;
        _serverService = serverService;
        _navigationService = navigationService;
        _context = context;

        _itemsSource.Connect()
            .AutoRefreshOnObservable(x => x.WhenAnyValue(i => i.Updated)) // <-- ключевой момент
            .Sort(SortExpressionComparer<ScannedInventoryListItem>.Descending(x => x.Updated))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(DetailItems)
            .Subscribe()
            .DisposeWith(D);

        _searchedItemsSource.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(SearchedItems)
            .Subscribe()
            .DisposeWith(D);

        SyncDetails = CreateSyncDetailsCommand();
        SearchBarCommand = CreateSearchBarCommand();
        SaveEntry = CreateSaveEntryCommand();

        SetupBarcodeAutoSave();
    }

    [ObservableProperty] public int _inventoryDataId;

    [ObservableProperty] public decimal _checkedQuantity;

    [ObservableProperty] public string _docNr;

    [ObservableProperty] public string _searchedText;

    [ObservableProperty] public bool _isKeyboardEnabled;

    public event Action<ScannedInventoryListItem>? ScrollToRequested;

    public event Action? FocusSearchRequested;

    public ObservableCollectionExtended<ScannedInventoryListItem> DetailItems { get; } = new();

    public ObservableCollectionExtended<InventoryListItem> SearchedItems { get; set; } = new();

    public ScannedInventoryListItem? LastUpdatedItem { get; private set; }

    public ReactiveCommand<Unit, Unit> SyncDetails { get; }

    public ReactiveCommand<string, Unit> SearchBarCommand { get; }

    public ReactiveCommand<ScannedInventoryListItem, Unit> SaveEntry { get; }

    private void SetupBarcodeAutoSave()
    {
        this.WhenAnyValue(x => x.SearchedText)
            .Where(barcode => !string.IsNullOrWhiteSpace(barcode) && barcode.Length == 13)
            .Throttle(TimeSpan.FromMilliseconds(140))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(async barcode =>
            {
                await HandleBarcodeScanned(barcode);
            })
            .DisposeWith(D);
    }

    private async Task HandleBarcodeScanned(string barcode)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                var existingItem = DetailItems.FirstOrDefault(x => x.Barcode == barcode);

                if (existingItem != null)
                {
                    existingItem.CheckedQuantity += 1;

                    // Execute the command on the main thread
                    await SaveEntry.Execute(existingItem);
                }
                else
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Barcode not found in current inventory")
                        .SetBackgroundColor(System.Drawing.Color.Orange)
                        .SetPosition(ToastPosition.Top));
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Toast(new ToastConfig($"Error processing barcode: {ex.Message}")
                    .SetBackgroundColor(System.Drawing.Color.Red)
                    .SetPosition(ToastPosition.Top));
            }
            finally
            {
                SearchedText = string.Empty;
                FocusSearchRequested?.Invoke();
            }
        });
    }

    private ReactiveCommand<string, Unit> CreateSearchBarCommand()
    {
        return ReactiveCommand
            .CreateFromTask<string>(async z =>
            {
                try
                {
                    if (z.Length != 13)
                    {
                        var listItems = await SaveCheckedQuantityRecords(z).ConfigureAwait(false);

                        if (z.Count() > 3)
                        {
                            _itemsSource.Edit(innerList =>
                            {
                                innerList.Clear();
                                innerList.AddOrUpdate(listItems);
                            });
                        }
                        else
                        {
                            _itemsSource.Edit(innerList =>
                            {
                                innerList.AddOrUpdate(listItems);
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.Alert(ex.Message);
                }
            });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        InventoryDataId = (int)query["InventoryDataId"];
    }

    public override async Task LoadAsync()
    {
        if (InventoryDataId > 0)
            Preferences.Set("InventoryDataId", InventoryDataId);
        else
            InventoryDataId = Convert.ToInt32(Preferences.Get("InventoryDataId", 0));

        try
        {
            UserDialogs.Instance.ShowLoading("Loading");

            var docTask = _inventoryDataRepository.GetInventoryDataByIdAsync(InventoryDataId);
            var itemsTask = _scannedInventoryDataRepository.GetDetListByDocIdAsync(InventoryDataId);

            await Task.WhenAll(docTask, itemsTask);

            var docItems = await docTask.ConfigureAwait(false);
            var items = await itemsTask.ConfigureAwait(false);

            DocNr = docItems?.Name ?? string.Empty;

            var listItems = items.Select((x, index) => new ScannedInventoryListItem(x.Id)
            {
                Nr = index + 1,
                Id = x.Id,
                ExternalId = x.ExternalId,
                Name = x.Name,
                Barcode = x.Barcode,
                Code = x.PluCode,
                Amount = x.Quantity,
                ItemNumber = x.ItemNumber,
                Unit = x.Unit,
                Updated = x.Updated,
                CheckedQuantity = x.CheckedQuantity,
                IsLocallyAdded = x.IsLocallyAdded,
                TotalCheckedQuantity = x.TotalCheckedQuantity
            });

            _itemsSource.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddOrUpdate(listItems);
            });

            UserDialogs.Instance.HideLoading();
        }
        catch (Exception ex)
        {
            UserDialogs.Instance.Toast(new ToastConfig($"Error! {ex.Message}")
                .SetBackgroundColor(System.Drawing.Color.Red)
                .SetPosition(ToastPosition.Top));
            throw new Exception(ex.Message);
        }
        finally
        {
            UserDialogs.Instance.HideLoading();
        }
    }


    [RelayCommand]
    private async Task NavigateToScannedDataAdd()
    {
        var canExecute = this.WhenAnyValue(x => x.InventoryDataId)
            .Select(x => x > 0)
            .ObserveOn(RxApp.MainThreadScheduler);

        await _navigationService.GoToScannedDataAdd(InventoryDataId);
    }

    public ReactiveCommand<ScannedInventoryListItem, Unit> CreateSaveEntryCommand()
    {
        return ReactiveCommand.CreateFromTask<ScannedInventoryListItem>(async item =>
        {
            if (item.CheckedQuantity == 0)
                return;

            try
            {
                UserDialogs.Instance.ShowLoading();

                await Task.Delay(10);

                if (!await _restService.CheckConnectionAsync(ApiIp))
                    return;

                var defaultItems = await _scannedInventoryDataRepository
                    .GetDetListByDocIdAsync(InventoryDataId)
                    .ConfigureAwait(false);

                var defaultItem = defaultItems.FirstOrDefault(x => x.Id == item.Key);
                var previousChecked = defaultItem?.CheckedQuantity ?? 0;

                var newTotalChecked = (previousChecked != item.CheckedQuantity)
                    ? item.TotalCheckedQuantity + item.CheckedQuantity
                    : item.TotalCheckedQuantity;

                var record = new ScannedInventoryDataRecord()
                {
                    Id = item.Key,
                    ExternalId = item.ExternalId,
                    Name = item.Name,
                    PluCode = item.Code,
                    Barcode = item.Barcode,
                    CheckedQuantity = 0,
                    Quantity = item.Amount,
                    ItemNumber = item.ItemNumber,
                    Unit = item.Unit,
                    InventoryDataId = InventoryDataId,
                    IsLocallyAdded = item.IsLocallyAdded,
                    Updated = DateTime.Now,
                    TotalCheckedQuantity = newTotalChecked
                };

                await _scannedInventoryDataRepository
                    .SaveNewAmountAsync(record)
                    .ConfigureAwait(false);

                await _inventoryDataRepository
                    .UpdateInventoryDocStatus(InventoryDataId, InventoryStatus.InProcess.ToString())
                    .ConfigureAwait(false);

                var employee2InventoryData = await _administrationRepository
                    .GetLatestEmployee2InventoryDataAsync()
                    .ConfigureAwait(false);

                var inventData = new InventoryDataRecord
                {
                    Id = InventoryDataId,
                    Employee1CheckerId = employee2InventoryData.Employee1Id,
                    Employee2CheckerId = employee2InventoryData.Employee2Id,
                    ScannedEmployeeId = employee2InventoryData.ScannedEmployeeId
                };

                var jsonDoc = JsonSerializer.Serialize(inventData);
                await _restService.SendInventoryDataAsync(jsonDoc);

                item.TotalCheckedQuantity = newTotalChecked;
                item.CheckedQuantity = 0;
                item.Updated = DateTime.Now;

                _itemsSource.Edit(updater => updater.AddOrUpdate(item));

                RaiseScrollTo(item);

                UserDialogs.Instance.Toast(
                    new ToastConfig("Values successfully saved!")
                        .SetBackgroundColor(System.Drawing.Color.Green)
                        .SetPosition(ToastPosition.Top));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Toast(
                    new ToastConfig($"Error! Values not saved! Please try again! {ex.Message}")
                        .SetBackgroundColor(System.Drawing.Color.Red)
                        .SetPosition(ToastPosition.Top));
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        });
    }

    public ReactiveCommand<Unit, Unit> CreateSyncDetailsCommand()
    {
        string result, jsonDetails;

        var canExecute = this.WhenAnyValue(x => x.DetailItems.Count)
            .Select(x => x > 0)
            .ObserveOn(RxApp.MainThreadScheduler);

        return ReactiveCommand
            .CreateFromTask(async () =>
            {

                var employee2InventoryData = await _administrationRepository.GetLatestEmployee2InventoryDataAsync()
                    .ConfigureAwait(false);

                var inventData = new InventoryDataRecord
                {
                    Id = InventoryDataId,
                    Employee1CheckerId = employee2InventoryData.Employee1Id,
                    Employee2CheckerId = employee2InventoryData.Employee2Id,
                    ScannedEmployeeId = employee2InventoryData.ScannedEmployeeId,
                    Status = "Checked",
                    Finished = DateTime.Now
                };

                var response = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig()
                    .UseYesNo()
                    .SetTitle("Please confirm your request")
                    .SetMessage("Are you sure?"));

                if (!response) return;

                UserDialogs.Instance.ShowLoading("Loading");

                if (!await _restService.CheckConnectionAsync(ApiIp))
                    return;

                var jsonDoc = JsonSerializer.Serialize(inventData);
                await _restService.SendInventoryDataAsync(jsonDoc);

                var newScannedDetails = DetailItems
                    .Where(x => x.IsLocallyAdded == true)
                    .Select(x => new ScannedInventoryDataRecord
                    {
                        Name = x.Name,
                        PluCode = x.Code,
                        Barcode = x.Barcode,
                        Quantity = 0,
                        ItemNumber = x.ItemNumber,
                        CheckedQuantity = x.TotalCheckedQuantity,
                        FinalQuantity = x.TotalCheckedQuantity,
                        Unit = x.Unit,
                        IsLocallyAdded = x.IsLocallyAdded,
                        IsExternal = true,
                        Created = DateTime.Now
                    }).ToList();

                jsonDetails = JsonSerializer.Serialize(newScannedDetails);

                await Task.Delay(50);

                result = await _restService.SendScannedDataInsertAsync(jsonDetails, InventoryDataId);

                if (string.IsNullOrEmpty(result))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Error! Data is not sent! Please try again!")
                        .SetBackgroundColor(System.Drawing.Color.Red)
                        .SetPosition(ToastPosition.Top));
                    return;
                }

                var scannedDetails = DetailItems.Select(x => new ScannedInventoryDataRecord
                {
                    Id = x.ExternalId,
                    CheckedQuantity = x.TotalCheckedQuantity,
                    FinalQuantity = x.TotalCheckedQuantity,
                    IsLocallyAdded = x.IsLocallyAdded
                }).Where(x => x.IsLocallyAdded is false);

                jsonDetails = JsonSerializer.Serialize(scannedDetails);
                result = await _restService.SendScannedDataUpdateAsync(jsonDetails, InventoryDataId);

                if (string.IsNullOrEmpty(result))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Error! Data is not sent! Please try again!")
                        .SetBackgroundColor(System.Drawing.Color.Red)
                        .SetPosition(ToastPosition.Top));
                    return;
                }

                await _administrationRepository.UpdateInventoryDataAsync(new Employee2InventoryDataRecord
                {
                    InventDataId = InventoryDataId
                }).ConfigureAwait(false);

                await _context.InsertItemAsync(new Employee2InventoryDataRecord
                {
                    Employee1Id = employee2InventoryData.Employee1Id,
                    Employee2Id = employee2InventoryData.Employee2Id,
                    ScannedEmployeeId = employee2InventoryData.ScannedEmployeeId
                }).ConfigureAwait(false);

                await _inventoryDataRepository.UpdateInventoryDocStatus(InventoryDataId, InventoryStatus.Checked.ToString())
                    .ConfigureAwait(false);

                UserDialogs.Instance.Toast(new ToastConfig("Data successfully synchronized!")
                    .SetBackgroundColor(System.Drawing.Color.Green)
                    .SetPosition(ToastPosition.Top));

                await SyncDataFromServer();

                var allScannedDetails = DetailItems
                    .Select(x => new ScannedInventoryDataRecord
                    {
                        Name = x.Name,
                        PluCode = x.Code,
                        Barcode = x.Barcode,
                        Quantity = 0,
                        ItemNumber = x.ItemNumber,
                        CheckedQuantity = x.TotalCheckedQuantity,
                        FinalQuantity = x.TotalCheckedQuantity,
                        Unit = x.Unit,
                        IsLocallyAdded = x.IsLocallyAdded,
                        IsExternal = true,
                        Created = DateTime.Now
                    });

                try
                {
                    var docId = await _restService.CreateDocumentAfterInventoryCheck(jsonDoc);
                    jsonDetails = JsonSerializer.Serialize(allScannedDetails);
                    await _restService.CreateDetailsAfterInventoryCheck(jsonDetails, docId);

                    await _context.DeleteAllAsync<ScannedInventoryDataRecord>().ConfigureAwait(false);
                    await _context.DeleteAllAsync<InventoryDataRecord>().ConfigureAwait(false);
                    await _context.DeleteAllAsync<EmployeeRecord>().ConfigureAwait(false);
                    await _context.DeleteAllAsync<Employee2InventoryDataRecord>().ConfigureAwait(false);

                    await MainThread.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        await _navigationService.GoToMainWindow();
                    });

                    UserDialogs.Instance.Toast(new ToastConfig("Data successfully synchronized!")
                        .SetBackgroundColor(System.Drawing.Color.Green)
                        .SetPosition(ToastPosition.Top));
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.Toast(new ToastConfig(ex.Message)
                        .SetBackgroundColor(System.Drawing.Color.Red)
                        .SetPosition(ToastPosition.Top));
                    return;
                }
                finally
                {
                    UserDialogs.Instance.HideLoading();
                }
            }, canExecute);
    }

    internal async Task SyncDataFromServer()
    {
        try
        {
            var docs = await _serverService.GetInventoryDataFromServer();
            if (docs.Count > 0)
            {
                await _serverService.GetScannedDataFromServer();
                await UpdateServerSyncDateTimeData();
                UserDialogs.Instance.Toast(new ToastConfig("Inventory data successfully synced!")
                    .SetBackgroundColor(System.Drawing.Color.Green)
                    .SetPosition(ToastPosition.Top));
            }
            else
            {
                UserDialogs.Instance.Toast(new ToastConfig("All data is up to date!")
                    .SetPosition(ToastPosition.Top));
            }
        }
        catch (Exception ex)
        {
            UserDialogs.Instance.Toast(new ToastConfig("Error! Inventory data not found!")
                .SetBackgroundColor(System.Drawing.Color.Red)
                .SetPosition(ToastPosition.Top));
            throw new Exception(ex.Message);
        }
    }
    internal async Task UpdateServerSyncDateTimeData()
    {
        await _administrationRepository.UpdateServerSyncDateTimeAsync(new ServerSyncDateTimeDataRecord
            {
                TableName = "tblInventoryData",
                LastSyncDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            })
            .ConfigureAwait(false);

        await _administrationRepository.UpdateServerSyncDateTimeAsync(new ServerSyncDateTimeDataRecord
            {
                TableName = "tblScannedData",
                LastSyncDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            })
            .ConfigureAwait(false);
    }

    internal async Task<IEnumerable<ScannedInventoryListItem>> SaveCheckedQuantityRecords(string letters)
    {
        //int indexer = 1;
        IReadOnlyList<ScannedInventoryDataRecord> items;

        if (letters.Count() > 3)
            items = await _scannedInventoryDataRepository.GetDetListByDocIdAndByDetNameAsync(InventoryDataId, SearchedText)
                .ConfigureAwait(false);
        else
            items = await _scannedInventoryDataRepository.GetDetListByDocIdAsync(InventoryDataId)
                .ConfigureAwait(false);

        var listItems = items
            .Select((x, index) => new ScannedInventoryListItem(x.Id)
            {
                Nr = index + 1,
                ExternalId = x.ExternalId,
                Name = x.Name,
                Barcode = x.Barcode,
                Code = x.PluCode,
                Amount = x.Quantity,
                ItemNumber = x.ItemNumber,
                Unit = x.Unit,
                Updated = x.Updated,
                CheckedQuantity = x.CheckedQuantity,
                TotalCheckedQuantity = x.TotalCheckedQuantity
            })
            .OrderByDescending(x => x.Updated);

        return listItems;
    }

    internal InventoryDataRecord UpdateInventoryData(Employee2InventoryDataRecord employee2InventoryData)
    {
        return new InventoryDataRecord
        {
            Id = InventoryDataId,
            Employee1CheckerId = employee2InventoryData.Employee1Id,
            Employee2CheckerId = employee2InventoryData.Employee2Id,
            Status = InventoryStatus.Checked.ToString()
        };
    }
    private void RaiseScrollTo(ScannedInventoryListItem item)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ScrollToRequested?.Invoke(item);
        });
    }
}