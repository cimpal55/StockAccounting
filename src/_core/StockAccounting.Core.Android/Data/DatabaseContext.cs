using System.Linq.Expressions;
using Microsoft.Maui.Storage;
using SQLite;

namespace StockAccounting.Core.Android.Data
{
    public partial class DatabaseContext : IAsyncDisposable, IDisposable
    {
        private const string DatabaseName = "StockAccountingInventory.db3";

        private static readonly string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseName);

        private static readonly SQLiteAsyncConnection ConnectionPool = new(DatabasePath);

        protected SQLiteConnection OpenConnection() => new(DatabasePath);

        protected SQLiteAsyncConnection GetAsyncConnection() => ConnectionPool;

        public async ValueTask DisposeAsync() =>
            await GetAsyncConnection().CloseAsync();

        public void Dispose() =>
            OpenConnection().Close();

        private async Task CreateTableIfNotExists<TTable>() where TTable : class, new()
        {
            await ConnectionPool.CreateTableAsync<TTable>();
        }

        private async Task<AsyncTableQuery<TTable>> GetTableAsync<TTable>() where TTable : class, new()
        {
            await CreateTableIfNotExists<TTable>();
            return ConnectionPool.Table<TTable>();
        }

        private async Task<TResult> Execute<TTable, TResult>(Func<Task<TResult>> action) where TTable : class, new()
        {
            await CreateTableIfNotExists<TTable>();
            return await action();
        }

        public async Task<IEnumerable<TTable>> GetAllAsync<TTable>() where TTable : class, new()
        {
            var table = await GetTableAsync<TTable>();
            return await table.ToListAsync();
        }

        public async Task DeleteAllAsync<TTable>() where TTable : class, new()
        {
            await ConnectionPool.DeleteAllAsync<TTable>();
        }

        public async Task<IEnumerable<TTable>> GetFilteredAsync<TTable>(Expression<Func<TTable, bool>> predicate) where TTable : class, new()
        {
            var table = await GetTableAsync<TTable>();
            return await table.Where(predicate).ToListAsync();
        }

        public async Task<TTable> GetItemByKeyAsync<TTable>(object primaryKey) where TTable : class, new()
        {
            //await CreateTableIfNotExists<TTable>();
            //return await Database.GetAsync<TTable>(primaryKey);
            return await Execute<TTable, TTable>(async () => await ConnectionPool.GetAsync<TTable>(primaryKey));
        }

        public async Task<bool> InsertItemAsync<TTable>(TTable item) where TTable : class, new()
        {
            //await CreateTableIfNotExists<TTable>();
            //return await Database.InsertAsync(item) > 0;
            return await Execute<TTable, bool>(async () => await ConnectionPool.InsertAsync(item) > 0);
        }

        public async Task<bool> UpdateItemAsync<TTable>(TTable item) where TTable : class, new()
        {
            await CreateTableIfNotExists<TTable>();
            return await ConnectionPool.UpdateAsync(item) > 0;
        }

        public async Task<bool> DeleteItemAsync<TTable>(TTable item) where TTable : class, new()
        {
            await CreateTableIfNotExists<TTable>();
            return await ConnectionPool.DeleteAsync(item) > 0;
        }

        public async Task<bool> DeleteItemByKeyAsync<TTable>(object primaryKey) where TTable : class, new()
        {
            await CreateTableIfNotExists<TTable>();
            return await ConnectionPool.DeleteAsync<TTable>(primaryKey) > 0;
        }
    }
}
