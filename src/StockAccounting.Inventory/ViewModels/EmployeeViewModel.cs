using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Acr.UserDialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using StockAccounting.Core.Android.Data;
using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Core.Android.Models.DataTransferObjects;
using StockAccounting.Inventory.Models;
using StockAccounting.Inventory.Repositories.Interfaces;
using StockAccounting.Inventory.ViewModels.Base;

namespace StockAccounting.Inventory.ViewModels;

public partial class EmployeeViewModel : ViewModelBase
{
    private readonly SourceCache<EmployeeListItem, int> _employeesSource = new(static x => x.DisplayNameId);
    private readonly IAdministrationRepository _administrationRepository;
    private readonly DatabaseContext _context;
    private static readonly string _viewTitle = "Employees";
    public EmployeeViewModel(
        IAdministrationRepository administrationRepository, DatabaseContext context)
        : base(_viewTitle)
    {
        _administrationRepository = administrationRepository;
        _context = context;

        _employeesSource.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(Employees)
            .Subscribe()
            .DisposeWith(D);

        Save = CreateSaveCommand();
    }

    [ObservableProperty] public EmployeeListItem _selectedEmployee1;

    [ObservableProperty] public EmployeeListItem _selectedEmployee2;

    [ObservableProperty] public EmployeeListItem _selectedScannedEmployee;

    public ObservableCollectionExtended<EmployeeListItem> Employees { get; } = new();

    public ReactiveCommand<Unit, Unit> Save { get; }

    public override async Task LoadAsync()
    {
        var employeeItems = await _context.GetAllAsync<EmployeeRecord>()
            .ConfigureAwait(false);

        var employee2InventoryData = (await _context.GetFilteredAsync<Employee2InventoryDataRecord>
                (x => x.InventDataId == null)
            .ConfigureAwait(false))
            .Take(1)
            .FirstOrDefault();

        var employeeListItems = employeeItems
            .Select(static x => new EmployeeListItem(x.ExternalId)
            {
                DisplayName = $"{x.Name} {x.Surname} {x.Code}",
                Code = x.Code,
                DisplayNameId = x.ExternalId
            });

        _employeesSource.AddOrUpdate(employeeListItems.OrderBy(x => x.DisplayName));

        if (employee2InventoryData is not null)
        {
            SelectedEmployee1 =
                _employeesSource.Items.FirstOrDefault(x => x.DisplayNameId == employee2InventoryData.Employee1Id)!;
            SelectedEmployee2 =
                _employeesSource.Items.FirstOrDefault(x => x.DisplayNameId == employee2InventoryData.Employee2Id)!;
            SelectedScannedEmployee =
                _employeesSource.Items.FirstOrDefault(x => x.DisplayNameId == employee2InventoryData.ScannedEmployeeId)!;
        }
    }

    public ReactiveCommand<Unit, Unit> CreateSaveCommand()
    {

        var canExecute = this.WhenAnyValue(x => x.SelectedEmployee1, x => x.SelectedEmployee2, 
                x => x.SelectedScannedEmployee)
            .Select(x => x.Item1 != null && x.Item2 != null && x.Item3 != null)
            .ObserveOn(RxApp.MainThreadScheduler);

        return ReactiveCommand
            .CreateFromTask(async () =>
            {
                var item = new Employee2InventoryDataRecord
                {
                    Employee1Id = SelectedEmployee1!.DisplayNameId,
                    Employee2Id = SelectedEmployee2?.DisplayNameId,
                    ScannedEmployeeId = SelectedScannedEmployee!.DisplayNameId
                };

                var employee2InventoryData = await _administrationRepository.GetLatestEmployee2InventoryDataAsync()
                    .ConfigureAwait(false);

                if (employee2InventoryData is not null)
                {
                    await _administrationRepository.UpdateEmployeesAsync(item)
                        .ConfigureAwait(false);
                }
                else
                {
                    await _context.InsertItemAsync(item)
                        .ConfigureAwait(false);
                }

                UserDialogs.Instance.Toast(
                    new ToastConfig("Employees successfully saved!")
                        .SetBackgroundColor(System.Drawing.Color.Green)
                        .SetPosition(ToastPosition.Top));

                await Shell.Current.GoToAsync("..");
            }, canExecute);
    }
}