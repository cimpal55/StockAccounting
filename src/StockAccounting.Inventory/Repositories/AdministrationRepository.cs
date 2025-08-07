using StockAccounting.Core.Android.Data;
using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Core.Android.Models.DataTransferObjects;
using StockAccounting.Inventory.Repositories.Interfaces;

namespace StockAccounting.Inventory.Repositories
{
    public sealed class AdministrationRepository : DatabaseContext, IAdministrationRepository
    {
        public async Task<IReadOnlyList<EmployeeRecord>> GetEmployeesAsync() =>
            await GetAsyncConnection()
                .Table<EmployeeRecord>()
                .ToArrayAsync();

        public async Task<Employee2InventoryDataRecord> GetLatestEmployee2InventoryDataAsync() =>
            await GetAsyncConnection()
                .Table<Employee2InventoryDataRecord>()
                .Where(x => x.InventDataId == null)
                .Take(1)
                .FirstOrDefaultAsync();

        public async Task<List<ActiveEmployeesRecord>> GetLatestEmployeesAsync() =>
            await GetAsyncConnection()
            .QueryAsync<ActiveEmployeesRecord>(@"SELECT (SELECT Name || ' ' || Surname || ' ' || Code 
                                                 FROM tblEmployees WHERE e2i.Employee1Id = ExternalId) AS Employee1Name,
                                                (SELECT Name || ' ' || Surname || ' ' || Code FROM tblEmployees WHERE e2i.Employee2Id = ExternalId) AS Employee2Name, 
                                                (SELECT Name || ' ' || Surname || ' ' || Code FROM tblEmployees WHERE e2i.ScannedEmployeeId = ExternalId) AS ScannedEmployeeName
                                                FROM tblEmployees2InventoryData e2i WHERE InventDataId IS NULL")
            .ConfigureAwait(false);

        public async Task UpdateEmployeesAsync(Employee2InventoryDataRecord item) =>
            await GetAsyncConnection()
                .ExecuteAsync($"UPDATE tblEmployees2InventoryData SET Employee1Id = {item.Employee1Id}," +
                              $" Employee2Id = {item.Employee2Id}," +
                              $" ScannedEmployeeId = {item.ScannedEmployeeId} WHERE InventDataId IS NULL")
                .ConfigureAwait(false);

        public async Task UpdateInventoryDataAsync(Employee2InventoryDataRecord item) =>
            await GetAsyncConnection()
                .ExecuteAsync($"UPDATE tblEmployees2InventoryData SET InventDataId = {item.InventDataId} WHERE InventDataId IS NULL")
                .ConfigureAwait(false);

        public async Task UpdateServerSyncDateTimeAsync(ServerSyncDateTimeDataRecord item) =>
            await GetAsyncConnection()
                .ExecuteAsync($"UPDATE tblServerSyncDateTimeData SET LastSyncDateTime = '{item.LastSyncDateTime}' WHERE TableName = '{item.TableName}'")
                .ConfigureAwait(false);

        public async Task<ServerSyncDateTimeDataRecord> GetLatestServerSyncDateTimeAsync(string tableName) =>
            await GetAsyncConnection()
                .Table<ServerSyncDateTimeDataRecord>()
                .Where(x => x.TableName == tableName)
                .Take(1)
                .FirstOrDefaultAsync();

    }
}
