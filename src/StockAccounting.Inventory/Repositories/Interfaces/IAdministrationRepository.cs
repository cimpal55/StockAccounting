using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Core.Android.Models.DataTransferObjects;

namespace StockAccounting.Inventory.Repositories.Interfaces
{
    public interface IAdministrationRepository
    {
        Task<IReadOnlyList<EmployeeRecord>> GetEmployeesAsync();

        Task<Employee2InventoryDataRecord> GetLatestEmployee2InventoryDataAsync();

        Task UpdateEmployeesAsync(Employee2InventoryDataRecord item);

        Task UpdateInventoryDataAsync(Employee2InventoryDataRecord item);

        Task UpdateServerSyncDateTimeAsync(ServerSyncDateTimeDataRecord item);

        Task<ServerSyncDateTimeDataRecord> GetLatestServerSyncDateTimeAsync(string tableName);

        Task<List<ActiveEmployeesRecord>> GetLatestEmployeesAsync();
    }
}
