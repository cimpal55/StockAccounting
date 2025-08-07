using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Sextant;
using Splat;
using StockAccounting.Core.Android.Data;
using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Inventory.Repositories.Interfaces;
using StockAccounting.Inventory.Services.Interfaces;
using StockAccounting.Inventory.Utility;
using StockAccounting.Inventory.ViewModels.Base;
using StockAccounting.Inventory.Views;

namespace StockAccounting.Inventory.ViewModels
{
    public partial class ScannedInventoryDataAddViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly IScannedInventoryDataRepository _scannedDataRepository;
        private IDisposable _autoSaveSubscription;
        private readonly IRestService _restService;
        private readonly DatabaseContext _context;
        private static string _viewTitle = "AddDetail";

        public ScannedInventoryDataAddViewModel(
            IScannedInventoryDataRepository scannedDataRepository,
            DatabaseContext context,
            IRestService restService)
            : base(_viewTitle)
        {
            _scannedDataRepository = scannedDataRepository;
            _context = context;
            _restService = restService;

            AddNewDetail = CreateAddNewDetailCommand();
            SaveDetails = CreateSaveNewDetailCommand();
            DeleteRow = CreateDeleteRowCommand();

            SetupAutoSave();
        }

        [ObservableProperty] public int _inventoryDataId;

        [ObservableProperty] public string _barcode;

        [ObservableProperty] public ObservableCollection<ExternalDataRecord> _scannedStockDataList = [];

        [ObservableProperty] public bool _isKeyboardEnabled = false;

        public ReactiveCommand<Unit, Unit> AddNewDetail { get; }

        public ReactiveCommand<Unit, Unit> SaveDetails { get; }

        public ReactiveCommand<ExternalDataRecord, Unit> DeleteRow { get; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            InventoryDataId = (int)query["InventoryDataId"];
        }

        private void SetupAutoSave()
        {
            _autoSaveSubscription?.Dispose();

            _autoSaveSubscription = this.WhenAnyValue(x => x.Barcode)
                .Where(barcode => !string.IsNullOrWhiteSpace(barcode))
                .Do(barcode => Debug.WriteLine($"Barcode input: {barcode} (Length: {barcode.Length})"))
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Where(barcode => barcode.Length == 13)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async _ =>
                {
                    try
                    {
                        var canExecute = await AddNewDetail.CanExecute.FirstAsync();
                        if (canExecute)
                        {
                            await AddNewDetail.Execute();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"AutoSave error: {ex.Message}");
                    }
                });
        }

        public ReactiveCommand<Unit, Unit> CreateAddNewDetailCommand()
        {
            return ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    var externalData = await _restService.GetExternalDataIdByBarcode(Barcode).ConfigureAwait(false);

                    var existing = ScannedStockDataList.FirstOrDefault(x => x.Id == externalData.Id);

                    if (existing != null)
                    {
                        var index = ScannedStockDataList.IndexOf(existing);
                        if (index >= 0)
                        {
                            var updatedDevice = new ExternalDataRecord
                            {
                                Id = existing.Id,
                                Name = existing.Name,
                                Unit = existing.Unit,
                                ItemNumber = existing.ItemNumber,
                                PluCode = existing.PluCode,
                                Barcode = existing.Barcode,
                                CheckedQuantity = existing.CheckedQuantity + 1,
                                Updated = DateTime.Now
                            };

                            ScannedStockDataList.RemoveAt(index);
                            ScannedStockDataList.Insert(index, updatedDevice);
                        }
                    }
                    else
                    {
                        externalData.CheckedQuantity = 1;
                        externalData.Updated = DateTime.Now;
                        ScannedStockDataList.Add(externalData);
                    }

                    UserDialogs.Instance.Toast(new ToastConfig("Detail successfully added!")
                        .SetDuration(500)
                        .SetBackgroundColor(System.Drawing.Color.Green));
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.Alert("Barcode doesn't exist or an error occurred", "Error", "OK");
                }
                finally
                {
                    Barcode = string.Empty;
                }
            });
        }

        public ReactiveCommand<ExternalDataRecord, Unit> CreateDeleteRowCommand()
        {
            return ReactiveCommand.CreateFromTask<ExternalDataRecord>(async row =>
            {
                var answer = await UserDialogs.Instance.ConfirmAsync("Are you sure?", null, "Yes", "No");
                if (!answer)
                    return;

                ScannedStockDataList.Remove(row);
            });
        }

        public ReactiveCommand<Unit, Unit> CreateSaveNewDetailCommand()
        {
            var canExecute = ScannedStockDataList
                .ToObservableChangeSet()
                .Select(_ => ScannedStockDataList.Any())
                .ObserveOn(RxApp.MainThreadScheduler);

            return ReactiveCommand
                .CreateFromTask(async () =>
                {
                    foreach (var item in ScannedStockDataList)
                    {
                        var record = new ScannedInventoryDataRecord()
                        {
                            ExternalId = item.Id,
                            Name = item.Name,
                            ItemNumber = item.ItemNumber,
                            PluCode = item.PluCode,
                            Unit = item.Unit,
                            Barcode = item.Barcode,
                            Quantity = 0,
                            CheckedQuantity = 0,
                            FinalQuantity = 0,
                            TotalCheckedQuantity = item.CheckedQuantity,
                            InventoryDataId = InventoryDataId,
                            IsLocallyAdded = true,
                            Created = DateTime.Now,
                            Updated = DateTime.Now
                        };

                        await _scannedDataRepository.InsertOrUpdateAsync(record)
                            .ConfigureAwait(false);
                    }

                    ScannedStockDataList.Clear();

                    UserDialogs.Instance.Toast(new ToastConfig("Detail successfully added!")
                        .SetBackgroundColor(System.Drawing.Color.Green));

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.GoToAsync("..");
                    });

                }, canExecute);
        }
    }
}
