using Acr.UserDialogs;
using DynamicData;
using DynamicData.Binding;
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
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace StockAccounting.Checklist.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IInventoryDataService _inventoryDataService;
        private readonly IEmployeeDataService _employeeDataService;
        private readonly IExternalDataService _externalDataService;
        private ObservableCollection<InventoryDataModel> _inventoryDataList;
        private ObservableCollection<EmployeeDataModel> _employeeDataList;
        private ObservableCollection<ExternalDataModel> _externalDataList;
        private ObservableCollection<ExternalDataModel> _scannedStockDataList;
        private InventoryDataModel _inventoryModel;
        private EmployeeDataModel _firstSelectedEmployee;
        private EmployeeDataModel _secondSelectedEmployee;
        private string _barcode;
        private string _firstEmployee;
        private string _secondEmployee;

        public MainViewModel(IDialogService dialogService,
            IInventoryDataService inventoryDataService,
            IEmployeeDataService employeeDataService,
            IExternalDataService externalDataService)
            : base(dialogService)
        {
            _inventoryDataService = inventoryDataService;
            _employeeDataService = employeeDataService;
            _externalDataService = externalDataService;

            Task.Run(() => this.OnAppearing()).Wait();
        }

        public ICommand InsertInventoryDataCommand => new Command(InsertInventoryData);
        public ICommand InitializeBarcodeScannerCommand => new Command<BarcodeScannerMode>(InitializeBarcodeScanner);
        public ICommand ButtonCommand => new Command<object>(OpenPickerPopup);
        public ICommand DeleteRowCommand => new Command(DeleteRowAsync);

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

        public ObservableCollection<ExternalDataModel> ScannedStockDataList
        {
            get => _scannedStockDataList;
            set
            {
                _scannedStockDataList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ExternalDataModel> ShownScannedDataList
        {
            get
            {
                if (ScannedStockDataList.Any())
                    return ScannedStockDataList;
                return null;
            }
        }
        public InventoryDataModel InventoryModel
        {
            get => _inventoryModel;
            set
            {
                _inventoryModel = value;
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
                    
                    InitializeBarcodeScanner(BarcodeScannerMode.Inventory);
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

        public async Task OnAppearing()
        {
            var employees = await GetEmployeeData();
            foreach (var item in employees)
            {
                item.FullName = string.Join(" ", item.Name, item.Surname, item.Code);
            }

            EmployeeDataList = employees.OrderBy(x => x.Name).ToObservableCollection();
            ExternalDataList = await GetExternalData();

            ScannedStockDataList = Preferences.ScannedStockDataList == null ? ScannedStockDataList = new ObservableCollection<ExternalDataModel>()
                                                                            : ScannedStockDataList = Preferences.ScannedStockDataList;
            FirstSelectedEmployee = Preferences.FirstSelectedEmployee;
            SecondSelectedEmployee = Preferences.SecondSelectedEmployee;
        }
        private async Task<ObservableCollection<EmployeeDataModel>> GetEmployeeData() => await _employeeDataService.GetEmployeeDataAsync();
        private async Task<ObservableCollection<ExternalDataModel>> GetExternalData() => await _externalDataService.GetExternalDataAsync();
        private async void InsertInventoryData()
        {
            if(FirstSelectedEmployee != null && SecondSelectedEmployee != null && ScannedStockDataList.Any())
            {
                if (ScannedStockDataList.Any(x => x.Quantity == 0) || ScannedStockDataList.Any(x => x.Quantity.Equals(null)))
                {
                    UserDialogs.Instance.Alert("Scanned data quantity can't be 0.", "Error", "OK");
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
                    scannedData = ScannedStockDataList
                };

                try
                {
                    await _inventoryDataService.InsertInventoryData(data);
                    CleanData();
                    await _dialogService.ShowDialog("Data inserted successfully", "Success", "OK");
                }
                catch (Exception e)
                {
                    await _dialogService.ShowDialog(e.ToString(), "Error", "OK");
                }
            }
            else
                UserDialogs.Instance.Alert("Missing some information", "Error", "OK");


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

        private void InitializeBarcodeScanner(BarcodeScannerMode mode)
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
                    ScanningData();
                    Preferences.ScannedStockDataList = ScannedStockDataList;
                    break;
            }
        }
        private void ScanningData()
        {
            try
            {
                var stock = ExternalDataList.First(x => x.Barcode == Barcode);
                if (!ScannedStockDataList.Any(x => x.Barcode == Barcode))
                {
                    stock.Quantity = 1;
                    stock.Created = DateTime.Now;
                    stock.Updated = DateTime.Now;
                    ScannedStockDataList.Add(stock);
                    foreach (var item in ShownScannedDataList)
                    {
                        item.FullName = item.Name + "\n" + "(" + item.Unit + ")";
                    }
                    OnPropertyChanged("ShownScannedDataList");
                }
                else
                {
                    var device = ScannedStockDataList.Where(x => x.Barcode == Barcode).FirstOrDefault();
                    var newDevice = device;
                    if (string.IsNullOrEmpty(Convert.ToString(newDevice.Quantity)))
                        newDevice.Quantity = 0.001m;
                    newDevice.Quantity++;
                    newDevice.Updated = DateTime.Now;
                    ScannedStockDataList.Replace(device, newDevice);
                    OnPropertyChanged("ShownScannedDataList");
                }
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
            ScannedStockDataList.Clear();
            Preferences.ScannedStockDataList = ScannedStockDataList;
            Preferences.FirstSelectedEmployee = null;
            Preferences.SecondSelectedEmployee = null;
            OnPropertyChanged("ShownScannedDataList");
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
            OnPropertyChanged("ShownScannedDataList");
        }
    }
}
