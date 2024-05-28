using Acr.UserDialogs;
using DynamicData;
using DynamicData.Binding;
using StockAccounting.Checklist.Constants;
using StockAccounting.Checklist.Controls;
using StockAccounting.Checklist.Enums;
using StockAccounting.Checklist.Extensions;
using StockAccounting.Checklist.Models.Data;
using StockAccounting.Checklist.Models.Data.DataTransferObjects;
using StockAccounting.Checklist.Services.Interfaces;
using StockAccounting.Checklist.Utility.Models;
using StockAccounting.Checklist.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.KeyboardHelper;
using static Xamarin.Forms.Internals.GIFBitmap;

namespace StockAccounting.Checklist.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IInventoryDataService _inventoryDataService;
        private readonly IEmployeeDataService _employeeDataService;
        private readonly IExternalDataService _externalDataService;
        private readonly IToolkitDataService _toolkitDataService;
        private ObservableCollection<InventoryDataModel> _inventoryDataList;
        private ObservableCollection<EmployeeDataModel> _employeeDataList;
        private ObservableCollection<ExternalDataModel> _externalDataList;
        private ObservableCollection<ExternalDataModel> _scannedStockDataList;
        private ObservableCollection<ToolkitDataModel> _toolkitDataList;
        private ObservableCollection<ToolkitHistoryModel> _usedToolkistList;
        private EmployeeDataModel _firstSelectedEmployee;
        private EmployeeDataModel _secondSelectedEmployee;
        private string _barcode;
        private string _firstEmployee;
        private string _secondEmployee;
        private bool _isKeyboardEnabled;

        public MainViewModel(IDialogService dialogService,
            IInventoryDataService inventoryDataService,
            IEmployeeDataService employeeDataService,
            IExternalDataService externalDataService,
            IToolkitDataService toolkitDataService)
            : base(dialogService)
        {
            _toolkitDataService = toolkitDataService;
            _inventoryDataService = inventoryDataService;
            _employeeDataService = employeeDataService;
            _externalDataService = externalDataService;

            Task.Run(() => OnAppearing());
        }

        public ICommand InsertInventoryDataCommand => new Command(InsertInventoryData);
        public ICommand InitializeBarcodeScannerCommand => new Command<BarcodeScannerMode>(async(o) => await InitializeBarcodeScannerAsync(o));
        public ICommand ButtonCommand => new Command<object>(OpenPickerPopup);
        public ICommand DeleteRowCommand => new Command(DeleteRowAsync);
        public ICommand DeleteAllRowsCommand => new Command(DeleteAllRowsAsync);
        public ICommand SynchronizingDataCommand => new Command(SynchronizingData);
        public ICommand ChangeKeyboardVisibilityCommand => new Command<object>(ChangeKeyboardVisibility);


        //public ICommand InitializeScannerCommand => new Command<BarcodeScannerMode>(InitializeScanner);
        public ObservableCollection<InventoryDataModel> InventoryDataList
        {
            get => _inventoryDataList;
            set
            {
                _inventoryDataList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EmployeeDataModel> EmployeeDataList
        {
            get => _employeeDataList;
            set
            {
                _employeeDataList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ExternalDataModel> ExternalDataList
        {
            get => _externalDataList;
            set
            {
                _externalDataList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ToolkitDataModel> ToolkitDataList
        {
            get => _toolkitDataList;
            set
            {
                _toolkitDataList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ExternalDataModel> ScannedStockDataList
        {
            get => _scannedStockDataList;
            set
            {
                _scannedStockDataList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ToolkitHistoryModel> UsedToolkitList
        {
            get => _usedToolkistList;
            set
            {
                _usedToolkistList = value;
                OnPropertyChanged();
            }
        }

        public EmployeeDataModel FirstSelectedEmployee
        {
            get => _firstSelectedEmployee;
            set
            {
                _firstSelectedEmployee = value;
                if(_firstSelectedEmployee != null)
                {
                    if (_firstSelectedEmployee == SecondSelectedEmployee)
                    {
                        _firstSelectedEmployee = null;
                        FirstEmployee = string.Empty;
                        UserDialogs.Instance.Alert("Employee already used", "Employee duplicate", "OK");
                    }
                    else
                    {
                        FirstEmployee = _firstSelectedEmployee.FullName;
                    }
                }
                Preferences.FirstSelectedEmployee = value;
                OnPropertyChanged();
            }
        }

        public EmployeeDataModel SecondSelectedEmployee
        {
            get => _secondSelectedEmployee;
            set
            {
                _secondSelectedEmployee = value;
                if(_secondSelectedEmployee != null)
                {
                    if (_secondSelectedEmployee == FirstSelectedEmployee)
                    {
                        _secondSelectedEmployee = null;
                        SecondEmployee = string.Empty;
                        UserDialogs.Instance.Alert("Employee already used", "Employee duplicate", "OK");
                    }
                    else
                    {
                        SecondEmployee = _secondSelectedEmployee.FullName;
                    }
                }
                Preferences.SecondSelectedEmployee = value;
                OnPropertyChanged();
            }
        }

        public string Barcode
        {
            get => _barcode;
            set
            {
                _barcode = value;
                if (value.Length == 13)
                {
                    InitializeBarcodeScannerAsync(BarcodeScannerMode.Inventory);
                }
                OnPropertyChanged();
            }
        }

        public string FirstEmployee
        {
            get => _firstEmployee;
            set
            {
                _firstEmployee = value;
                OnPropertyChanged();
            }
        }

        public string SecondEmployee
        {
            get => _secondEmployee;
            set
            {
                _secondEmployee = value;
                OnPropertyChanged();
            }
        }

        public bool IsKeyboardEnabled
        {
            get => _isKeyboardEnabled;
            set
            {
                _isKeyboardEnabled = value;
                OnPropertyChanged();
            }
        }

        public void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("Loading");

            if (!CheckConnection(ApiConstants.BaseApiUrl))
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.Toast(new ToastConfig("Error! Please check internet connection!").SetBackgroundColor(Color.Red));
                return;
            }

            EmployeeDataList = Preferences.EmployeeDataList;
            ExternalDataList = Preferences.ExternalDataList;
            ToolkitDataList = Preferences.ToolkitDataList;
            ScannedStockDataList = Preferences.ScannedStockDataList == null ? ScannedStockDataList = new ObservableCollection<ExternalDataModel>()
                                                                            : ScannedStockDataList = Preferences.ScannedStockDataList;

            UsedToolkitList = Preferences.ToolkitHistoryList == null ? UsedToolkitList = new ObservableCollection<ToolkitHistoryModel>()
                                                                     : Preferences.ToolkitHistoryList;


            FirstSelectedEmployee = Preferences.FirstSelectedEmployee;
            SecondSelectedEmployee = Preferences.SecondSelectedEmployee;

            UserDialogs.Instance.HideLoading();
        }

        private async Task<ObservableCollection<EmployeeDataModel>> GetEmployeeData() => await _employeeDataService.GetEmployeeDataAsync();
        private async Task<ObservableCollection<ExternalDataModel>> GetExternalData() => await _externalDataService.GetExternalDataAsync();
        private async Task<ObservableCollection<ToolkitDataModel>> GetToolkitData() => await _toolkitDataService.GetToolkitDataAsync();

        public bool CheckConnection(string ip)
        {
            var pingSender = new Ping();
            var reply = pingSender.Send(ip);

            return reply == null || reply.Status == IPStatus.Success;
        }

        private async void InsertInventoryData()
        {
            Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Inserting data..."));

            if (FirstSelectedEmployee != null && SecondSelectedEmployee != null && ScannedStockDataList.Any())
            {
                if (ScannedStockDataList.Any(x => x.Quantity == 0) || ScannedStockDataList.Any(x => x.Quantity.Equals(null)))
                {
                    UserDialogs.Instance.Alert("Scanned data quantity can't be 0.", "Error", "OK");
                    UserDialogs.Instance.HideLoading();
                    return;
                }

                var inventoryModel = new InventoryDataModel()
                {
                    Employee1Id = FirstSelectedEmployee.Id,
                    Employee2Id = SecondSelectedEmployee.Id,
                };

                var data = new ScannedModel
                {
                    inventoryData = inventoryModel,
                    scannedData = ScannedStockDataList,
                    usedToolkitData = UsedToolkitList,
                };

                try
                {
                    await _inventoryDataService.InsertInventoryData(data);
                    CleanData();
                    UserDialogs.Instance.Alert("Data inserted successfully", "Success", "OK");
                }
                catch (Exception e)
                {
                    UserDialogs.Instance.Alert(e.Message, "Error", "OK");
                }
            }
            else
            {
                UserDialogs.Instance.Alert("Missing some information", "Error", "OK");
            }

            Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideLoading());
        }

        // Camera scanner

        //private async void InitializeScanner(BarcodeScannerMode mode)
        //{
        //    PermissionStatus granted = await Permissions.CheckStatusAsync<Permissions.Camera>();
        //    if (granted != PermissionStatus.Granted)
        //    {
        //        _ = await Permissions.RequestAsync<Permissions.Camera>();
        //    }
        //    if (granted == PermissionStatus.Granted)
        //    {
        //        try
        //        {
        //            MobileBarcodeScanner scanner = new MobileBarcodeScanner();
        //            ZXing.Result result = await scanner.Scan();
        //            if (result != null && result.Text != "")
        //            {
        //                var scannedData = result.ToString().Trim();

        //                switch (mode)
        //                {
        //                    case BarcodeScannerMode.First:
        //                        SelectedEmployee1 = EmployeeDataList.First(x => x.Code == scannedData);
        //                        break;
        //                    case BarcodeScannerMode.Second:
        //                        SelectedEmployee2 = EmployeeDataList.First(x => x.Code == scannedData);
        //                        break;
        //                    case BarcodeScannerMode.Inventory:
        //                        var stock = await _externalDataService.GetExternalDataByBarcodeAsync(scannedData);
        //                        stock.Quantity = 1;
        //                        if (!ScannedStockDataList.Any(x => x.Barcode == scannedData))
        //                            ScannedStockDataList.Add(stock);
        //                        break;
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.WriteLine(e);
        //        }
        //    }
        //    else
        //    {
        //        await _dialogService.ShowDialog("Problem", "No permissions to use camera.", "ΟΚ");
        //    }
        //}


        private async Task InitializeBarcodeScannerAsync(BarcodeScannerMode mode)
        {
            EmployeeDataModel firstEmployee;
            EmployeeDataModel secondEmployee;
            switch (mode)
            {
                case BarcodeScannerMode.First:                 
                    try
                    {
                        
                        firstEmployee = EmployeeDataList.First(x => x.Code == FirstEmployee);
                        FirstSelectedEmployee = firstEmployee; 
                    }
                    catch
                    {
                        FirstEmployee = string.Empty;
                        UserDialogs.Instance.Alert("Employee wasn't found", "Employee error", "OK");
                    }
                    break;

                case BarcodeScannerMode.Second:
                    try
                    {
                        secondEmployee = EmployeeDataList.First(x => x.Code == SecondEmployee);
                        SecondSelectedEmployee = secondEmployee;
                    }
                    catch
                    {
                        SecondEmployee = string.Empty;
                        UserDialogs.Instance.Alert("Employee wasn't found", "Employee error", "OK");
                    }
                    break;

                case BarcodeScannerMode.Inventory:
                    await ScanningDataAsync();
                    Preferences.ScannedStockDataList = ScannedStockDataList;
                    Preferences.ToolkitHistoryList = UsedToolkitList;
                    break;
            }
        }
        private async Task ScanningDataAsync()
        {
            try
            {
                if (!ToolkitDataList.Any(x => x.Barcode == Barcode))
                {
                    var stock = ExternalDataList.First(x => x.Barcode == Barcode);

                    if (!ScannedStockDataList.Any(x => x.Barcode == Barcode))
                    {
                        stock.Quantity = 1;
                        stock.Created = DateTime.Now;
                        stock.Updated = DateTime.Now;
                        ScannedStockDataList.Add(stock);
                        IsKeyboardEnabled = false;
                    }
                    else
                    {
                        var device = ScannedStockDataList.Where(x => x.Barcode == Barcode).FirstOrDefault();
                        var newDevice = device;
                        if (string.IsNullOrEmpty(Convert.ToString(newDevice.Quantity)))
                            newDevice.Quantity = 0.1m;
                        newDevice.Quantity++;
                        newDevice.Updated = DateTime.Now;
                        ScannedStockDataList.Replace(device, newDevice);
                        IsKeyboardEnabled = false;
                    }
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Inserting toolkit..."));
                    var toolkit = ToolkitDataList.FirstOrDefault(x => x.Barcode == Barcode);
                    var toolkitExternal = await _toolkitDataService.GetToolkitExternalData(toolkit.Id);
                    ExternalDataModel external = new ExternalDataModel();
                    foreach (var item in toolkitExternal)
                    {
                        external = ExternalDataList.First(x => x.Id == item.ExternalDataId);
                        if(!ScannedStockDataList.Any(x => x.Barcode == external.Barcode))
                        {
                            external.Quantity = item.Quantity;
                            external.Created = DateTime.Now;
                            external.Updated = DateTime.Now;
                            ScannedStockDataList.Add(external);
                        }
                        else
                        {
                            var device = ScannedStockDataList.Where(x => x.Barcode == external.Barcode).FirstOrDefault();
                            var newDevice = device;
                            if (string.IsNullOrEmpty(Convert.ToString(newDevice.Quantity)))
                                newDevice.Quantity = 0.1m;
                            newDevice.Quantity++;
                            newDevice.Updated = DateTime.Now;
                            ScannedStockDataList.Replace(device, newDevice);
                        }
                    }

                    ToolkitHistoryModel usedToolkit = new ToolkitHistoryModel
                    {
                        ToolkitId = toolkit.Id,
                    };

                    UsedToolkitList.Add(usedToolkit);
                    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideLoading());
                }

                ScannedStockDataList.Sort(x => x.Updated);
            }
            catch
            {
                UserDialogs.Instance.Alert("Barcode doesn't exist", "Product barcode", "OK");
            }

            Barcode = string.Empty;
        }

        private void CleanData()
        {
            FirstSelectedEmployee = null;
            SecondSelectedEmployee = null;
            FirstEmployee = null;
            SecondEmployee = null;

            ScannedStockDataList = new ObservableCollection<ExternalDataModel>();
            UsedToolkitList = new ObservableCollection<ToolkitHistoryModel>();
            
            Preferences.ScannedStockDataList = ScannedStockDataList;
            Preferences.ToolkitHistoryList = UsedToolkitList;
            Preferences.FirstSelectedEmployee = null;
            Preferences.SecondSelectedEmployee = null;
        }

        private void OpenPickerPopup(object obj)
        {
            var view = obj as Picker;
            view?.Focus();
        }

        private async void DeleteRowAsync(object obj)
        {
            var answer = await UserDialogs.Instance.ConfirmAsync("Are you sure?", null, "Yes", "No");
            if (!answer)
                return;
            var row = obj as ExternalDataModel;
            ScannedStockDataList.Remove(row);
            Preferences.ScannedStockDataList = ScannedStockDataList;
        }

        private async void DeleteAllRowsAsync()
        {
            var answer = await UserDialogs.Instance.ConfirmAsync("Are you sure?", null, "Yes", "No");
            if (!answer)
                return;
            ScannedStockDataList.Clear();
            Preferences.ScannedStockDataList = ScannedStockDataList;
        }

        private async void SynchronizingData()
        {
            UserDialogs.Instance.ShowLoading("Loading");
            await Task.Delay(100);

            try
            {
                if (!CheckConnection(ApiConstants.BaseApiUrl))
                {
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.Toast(new ToastConfig("Error! Please check internet connection!").SetBackgroundColor(Color.Red));
                    return;
                }

                var employees = await GetEmployeeData();
                EmployeeDataList = employees.OrderBy(x => x.Code).ToObservableCollection();
                ExternalDataList = await GetExternalData();
                ToolkitDataList = await GetToolkitData();

                Preferences.ExternalDataList = ExternalDataList;
                Preferences.ToolkitDataList = ToolkitDataList;
                Preferences.EmployeeDataList = EmployeeDataList;

                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.Toast(new ToastConfig("Synchronization was successful!").SetBackgroundColor(Color.Green));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.Toast(new ToastConfig("Error! Synchronization failed!").SetBackgroundColor(Color.Red));
                throw new Exception(ex.Message);
            }
        }

        private void ChangeKeyboardVisibility(object obj)
        {
            if (IsKeyboardEnabled)
                IsKeyboardEnabled = false;
            else
            {
                IsKeyboardEnabled = true;
                var entry = obj as Entry;
                entry.Focus();
            }
        }
    }
}
