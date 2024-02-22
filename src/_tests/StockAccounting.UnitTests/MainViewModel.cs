using Sextant;
using StockAccounting.Checklist.Extensions;
using StockAccounting.Checklist.Models.Data;
using StockAccounting.Checklist.Services.Interfaces;
using StockAccounting.Checklist.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace StockAccounting.UnitTests
{
    public class MainViewModel
    {
        private readonly IInventoryDataService _inventoryDataService;
        private readonly IEmployeeDataService _employeeDataService;
        private ObservableCollection<InventoryDataModel> _inventoryDataList;
        private ObservableCollection<EmployeeDataModel> _employeeDataList;
        private InventoryDataModel _inventoryModel;
        public MainViewModel(IInventoryDataService inventoryDataService,
            IEmployeeDataService employeeDataService)
        {
            _inventoryDataService = inventoryDataService;
            _employeeDataService = employeeDataService;
            _employeeDataList = new ObservableCollection<EmployeeDataModel>();
        }

        public ICommand InsertInventoryDataCommand => new Command<InventoryDataModel>(InsertInventoryData);

        public ObservableCollection<InventoryDataModel> InventoryDataList
        {
            get => _inventoryDataList;
            set
            {
                _inventoryDataList = value;
            }
        }
        public ObservableCollection<EmployeeDataModel> EmployeeDataList
        {
            get => _employeeDataList;
            set
            {
                _employeeDataList = value;
            }
        }
        public InventoryDataModel InventoryModel
        {
            get => _inventoryModel;
            set
            {
                _inventoryModel = value;
            }
        }
        public virtual async Task InitializeAsync(object data)
        {
            EmployeeDataList = (await _employeeDataService.GetEmployeeDataAsync()).ToObservableCollection();
            InventoryDataList = (await _inventoryDataService.GetInventoryDataAsync()).ToObservableCollection();
        }
        private async void InsertInventoryData(InventoryDataModel item)
        {
            //await _inventoryDataService.InsertInventoryData(item);
            //MessagingCenter.Send(this, "InventoryDataInserted");
            //await _dialogService.ShowDialog("Inventory data inserted successfully", "Success", "OK");
        }
    }
}
