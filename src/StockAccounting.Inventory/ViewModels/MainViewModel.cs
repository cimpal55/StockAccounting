using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using Acr.UserDialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using StockAccounting.Core.Android.Data;
using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Core.Android.Models.DataTransferObjects;
using StockAccounting.Core.Data.Models.Data.InventoryData;
using StockAccounting.Inventory.Repositories.Interfaces;
using StockAccounting.Inventory.Resources;
using StockAccounting.Inventory.Services.Interfaces;
using StockAccounting.Inventory.ViewModels.Base;
using Color = System.Drawing.Color;

namespace StockAccounting.Inventory.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IAdministrationRepository _administrationRepository;
        private readonly IInventoryDataRepository _inventoryDataRepository;
        private readonly IScannedInventoryDataRepository _scannedDataRepository;
        
        private readonly IRestService _restService;
        private readonly IServerService _serverService;
        private readonly DatabaseContext _context;

        private static readonly string ApiIp = Preferences.Get("ApiIp", "default_value");
        private static readonly string _viewTitle = "Main";

        public MainViewModel(
            INavigationService navigationService,
            IAdministrationRepository administrationRepository,
            IInventoryDataRepository inventoryDataRepository,
            IScannedInventoryDataRepository scannedDataRepository,
            IRestService restService, 
            DatabaseContext context, 
            IServerService serverService)
            : base(_viewTitle)
        {
            _navigationService = navigationService;
            _restService = restService;
            _serverService = serverService;

            _administrationRepository = administrationRepository;
            _inventoryDataRepository = inventoryDataRepository;
            _scannedDataRepository = scannedDataRepository;
            
            _context = context;

            NavigateToEmployee = CreateNavigateToEmployeeCommand();
            NavigateToInventory = CreateNavigateToInventoryCommand();
            GetInventoryData = CreateGetInventoryDataCommand();
            GetLatestData = CreateGetLatestDataCommand();
            DeleteAll = CreateDeleteAllCommand();
        }

        [ObservableProperty] private ObservableCollection<string> _items;

        [ObservableProperty] public int _netSuiteInventoryDataCount;

        [ObservableProperty] public int _netSuiteScannedDataCount;

        [ObservableProperty] public int _netSuiteEmployeesCount;

        [ObservableProperty] public string _netSuiteLastSyncData;

        [ObservableProperty] public bool _isLoadDataVisible;

        [ObservableProperty] public string _currentVersion;

        [ObservableProperty] public int _isEmployeeSet;

        [ObservableProperty] public string _selectedEmployees;

        [ObservableProperty] public bool _isStatusVisible;

        public ReactiveCommand<Unit, Unit> GetInventoryData { get; }

        public ReactiveCommand<Unit, Unit> NavigateToInventory { get; }

        public ReactiveCommand<Unit, Unit> NavigateToEmployee { get; }

        public ReactiveCommand<Unit, Unit> GetLatestData { get; }

        public ReactiveCommand<Unit, Unit> DeleteAll { get; }


        public override async Task LoadAsync()
        {
            CurrentVersion = VersionTracking.CurrentVersion;

            await SetCounting().ConfigureAwait(false);

            var employee2InventoryData = await _administrationRepository.GetLatestEmployee2InventoryDataAsync()
                .ConfigureAwait(false);

            IsEmployeeSet = (employee2InventoryData?.Employee1Id != null && employee2InventoryData?.ScannedEmployeeId != null) ? 1 : 0;

            if (IsEmployeeSet == 1)
                IsLoadDataVisible = false;
            else
                IsLoadDataVisible = true;

            IsStatusVisible = true;
        }

        public ReactiveCommand<Unit, Unit> CreateNavigateToEmployeeCommand()
        {
            var canExecute = this.WhenAnyValue(x => x.IsLoadDataVisible)
                .Select(x => x == false)
                .ObserveOn(RxApp.MainThreadScheduler);

            return ReactiveCommand
                .CreateFromTask(async () => await _navigationService.GoToEmployee(), canExecute);
        }

        public ReactiveCommand<Unit, Unit> CreateNavigateToInventoryCommand()
        {
            var canExecute = this.WhenAnyValue(x => x.IsEmployeeSet)
                .Select(x => x != 0)
                .ObserveOn(RxApp.MainThreadScheduler);

            string scannedEmployeeCode, jsonInventory, jsonDetails, inventoryName;
            int docId;

            return ReactiveCommand
                .CreateFromTask(async () =>
                {
                    try
                    {
                        UserDialogs.Instance.ShowLoading("Loading...");

                        var employees = await _administrationRepository.GetLatestEmployeesAsync()
                            .ConfigureAwait(false);

                        var inventoryData = await _inventoryDataRepository.GetFullListAsync()
                            .ConfigureAwait(false);

                        var scannedEmployee = employees[0].ScannedEmployeeName;
                        scannedEmployeeCode = scannedEmployee[^3..];

                        inventoryName = string.Format("Mašīna_{0}_{1:yyyy_MM_dd}", scannedEmployeeCode, DateTime.Now);

                        var existedInventoryId = await _restService.GetInventoryDataIdByName(inventoryName);

                        docId = existedInventoryId;
                        if (docId == 0)
                        {
                            var employee2InventoryData = await _administrationRepository
                                .GetLatestEmployee2InventoryDataAsync()
                                .ConfigureAwait(false);

                            var inventoryRecord = new InventoryDataModel
                            {
                                Employee1CheckerId = employee2InventoryData.Employee1Id,
                                Employee2CheckerId = employee2InventoryData.Employee2Id,
                                ScannedEmployeeId = employee2InventoryData.ScannedEmployeeId,
                                Name = inventoryName
                            };

                            jsonInventory = JsonSerializer.Serialize(inventoryRecord);

                            docId = await _restService.InsertInventoryDataAsync(jsonInventory)
                                .ConfigureAwait(false);

                            var details =
                                await _restService.GetDetailsByEmployeeId(employee2InventoryData.ScannedEmployeeId);

                            var finalList = details.Select(x => new ScannedInventoryDataRecord
                            {
                                Name = x.Name,
                                Barcode = x.Barcode,
                                PluCode = x.PluCode,
                                Quantity = x.Quantity,
                                IsExternal = false,
                                CheckedQuantity = 0,
                                FinalQuantity = 0,
                                Unit = x.Unit,
                                IsLocallyAdded = false,
                                ItemNumber = x.ItemNumber,
                                Created = DateTime.Now
                            });

                            jsonDetails = JsonSerializer.Serialize(finalList);
                            var result = await _restService.SendScannedDataInsertAsync(jsonDetails, docId);

                            await _serverService.GetInventoryDataFromServer();
                            await _serverService.GetScannedDataFromServer();
                        }

                        await MainThread.InvokeOnMainThreadAsync(
                            async () =>
                            {
                                await _navigationService.GoToScannedData(docId);
                            });
                    }
                    catch (Exception ex)
                    {
                        UserDialogs.Instance.Alert(ex.Message);
                    }
                    finally
                    {
                        UserDialogs.Instance.HideLoading();
                    }
                }, canExecute);
        }

        public ReactiveCommand<Unit, Unit> CreateGetInventoryDataCommand()
        {
            return ReactiveCommand
                .CreateFromTask(async () =>
                {
                    try
                    {
                        var response = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig()
                            .UseYesNo()
                            .SetTitle("Please confirm your request")
                            .SetMessage("Are you sure?"));
                        if (!response) return;

                        UserDialogs.Instance.ShowLoading("Loading");

                        if (!await _restService.CheckConnectionAsync(ApiIp))
                            return;

                        var inventoryData = await _serverService.GetInventoryDataFromServer();
                        NetSuiteInventoryDataCount = inventoryData.Count;

                        var scannedData = await _serverService.GetScannedDataFromServer();
                        NetSuiteScannedDataCount = scannedData.Count;

                        var employees = await _serverService.GetEmployeesFromServer();
                        NetSuiteEmployeesCount = employees.Count;

                        await SetServerSyncDateTime();

                        UserDialogs.Instance.Toast(new ToastConfig("Inventory data successfully saved!")
                            .SetBackgroundColor(System.Drawing.Color.Green)
                            .SetPosition(ToastPosition.Top));

                        IsLoadDataVisible = false;
                    }
                    catch (Exception ex)
                    {
                        UserDialogs.Instance.Toast(
                            new ToastConfig($"Error! {ex.Message}!")
                                .SetBackgroundColor(System.Drawing.Color.Red)
                                .SetPosition(ToastPosition.Top));
                    }
                    finally
                    {
                        UserDialogs.Instance.HideLoading();
                    }
                });
        }

        public ReactiveCommand<Unit, Unit> CreateGetLatestDataCommand()
        {
            var canExecute = this.WhenAnyValue(x => x.NetSuiteInventoryDataCount)
                .Select(x => x > 0)
                .ObserveOn(RxApp.MainThreadScheduler);

            return ReactiveCommand
                .CreateFromTask(async () =>
                {
                    try
                    {
                        var response = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig()
                            .UseYesNo()
                            .SetTitle("Please confirm your request")
                            .SetMessage("Are you sure?"));
                        if (!response) return;

                        UserDialogs.Instance.ShowLoading("Loading");

                        if (!await _restService.CheckConnectionAsync(ApiIp))
                            return;

                        var docs = await _serverService.GetLatestInventoryDataFromServer();
                        if (docs.Count > 0)
                        {
                            await _serverService.GetLatestScannedDataFromServer(docs)
                                .ConfigureAwait(false);
                            await UpdateServerSyncDateTimeData()
                                .ConfigureAwait(false);
                            await UpdateServerSyncDateTimeData()
                                .ConfigureAwait(false);

                            var inventoryData = await _inventoryDataRepository.GetListAsync()
                                .ConfigureAwait(false);
                            NetSuiteInventoryDataCount = inventoryData.Count;

                            UserDialogs.Instance.HideLoading();
                            UserDialogs.Instance.Toast(new ToastConfig("Inventory data successfully synced!")
                                .SetBackgroundColor(System.Drawing.Color.Green)
                                .SetPosition(ToastPosition.Top));

                        }
                        else
                        {
                            UserDialogs.Instance.HideLoading();
                            UserDialogs.Instance.Toast(new ToastConfig("All data is up to date!")
                                .SetPosition(ToastPosition.Top));
                        }

                        await UpdateInventoryDataStatus()
                            .ConfigureAwait(false);

                        await DeleteCheckedInventoryData()
                            .ConfigureAwait(false);

                        IsLoadDataVisible = false;
                    }
                    catch (Exception ex)
                    {
                        UserDialogs.Instance.Toast(
                            new ToastConfig($"Error! {ex.Message}!")
                                .SetBackgroundColor(System.Drawing.Color.Red)
                                .SetPosition(ToastPosition.Top));
                    }
                    finally
                    {
                        UserDialogs.Instance.HideLoading();
                    }
                }, canExecute);
        }

        public ReactiveCommand<Unit, Unit> CreateDeleteAllCommand()
        {
            var canExecute = this.WhenAnyValue(x => x.IsEmployeeSet)
                .Select(x => x != 0)
                .ObserveOn(RxApp.MainThreadScheduler);

            return ReactiveCommand
                .CreateFromTask(async () =>
                    {
                        var response = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig()
                            .UseYesNo()
                            .SetTitle("Please confirm your request")
                            .SetMessage("Are you sure?"));

                        if (!response) return;

                        UserDialogs.Instance.ShowLoading("Loading");

                        await _context.DeleteAllAsync<ScannedInventoryDataRecord>().ConfigureAwait(false);
                        await _context.DeleteAllAsync<InventoryDataRecord>().ConfigureAwait(false);
                        await _context.DeleteAllAsync<EmployeeRecord>().ConfigureAwait(false);
                        await _context.DeleteAllAsync<Employee2InventoryDataRecord>().ConfigureAwait(false);

                        var scannedDataCounter = await _scannedDataRepository.GetFullListAsync()
                            .ConfigureAwait(false);

                        var intData = await _inventoryDataRepository.GetFullListAsync()
                            .ConfigureAwait(false);

                        await SetCounting().ConfigureAwait(false);

                        UserDialogs.Instance.HideLoading();
                        UserDialogs.Instance.Toast(new ToastConfig("App data successfully deleted!")
                            .SetBackgroundColor(System.Drawing.Color.Green)
                            .SetPosition(ToastPosition.Top));

                        IsEmployeeSet = 0;
                        IsLoadDataVisible = true;
                    },
                    canExecute);
        }

        internal async Task DeleteCheckedInventoryData()
        {
            var items = await _restService.GetCheckedInventoryDataAsync();

            if (items is { Count: > 0 })
            {
                foreach (var item in items)
                {
                    await _inventoryDataRepository.DeleteCheckedInventoryData(item.Id)
                        .ConfigureAwait(false);

                    await _scannedDataRepository.DeleteCheckedScannedData(item.Id)
                        .ConfigureAwait(false);
                }
            }

            IsEmployeeSet = 0;
        }

        internal async Task UpdateInventoryDataStatus()
        {
            var items = await _restService.GetInprocessInventoryDataAsync();
            if (items is { Count: > 0 })
            {
                foreach (var item in items)
                {
                    await _inventoryDataRepository.UpdateInventoryDocStatus(item.Id, item.Status)
                        .ConfigureAwait(false);
                }
            }
        }

        internal async Task SetCounting()
        {
            var scannedData = await _scannedDataRepository.GetFullListAsync()
                .ConfigureAwait(false);

            var inventoryData = await _inventoryDataRepository.GetFullListAsync()
                .ConfigureAwait(false);

            var employee = await _administrationRepository.GetEmployeesAsync()
                .ConfigureAwait(false);

            var employees = await _administrationRepository.GetLatestEmployeesAsync()
                .ConfigureAwait(false);

            var lastSyncData = await _administrationRepository.GetLatestServerSyncDateTimeAsync("tblInventoryData")
                .ConfigureAwait(false);

            NetSuiteScannedDataCount = scannedData.Count;
            NetSuiteInventoryDataCount = inventoryData.Count;
            NetSuiteEmployeesCount = employee.Count;
            NetSuiteLastSyncData = lastSyncData is not null ? lastSyncData.LastSyncDateTime : string.Empty;
            SelectedEmployees = (employees.Count > 0) ? 
                $"{employees[0].Employee1Name} {employees[0].Employee2Name} {employees[0].ScannedEmployeeName}" 
                : string.Empty;
        }

        internal async Task SetServerSyncDateTime()
        {
            await _context.InsertItemAsync(new ServerSyncDateTimeDataRecord
            {
                TableName = "tblInventoryData",
                LastSyncDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            })
            .ConfigureAwait(false);

            await _context.InsertItemAsync(new ServerSyncDateTimeDataRecord
            {
                TableName = "tblScannedData",
                LastSyncDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            })
            .ConfigureAwait(false);

            await _context.InsertItemAsync(new ServerSyncDateTimeDataRecord
            {
                TableName = "tblEmployees",
                LastSyncDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            })
            .ConfigureAwait(false);

            NetSuiteLastSyncData = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
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


    }
}
