using LinqToDB;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.Core.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDataConnection _conn;
        public GenericRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public async Task<int> InsertAsync(T obj) =>
            await _conn
                .InsertWithInt32IdentityAsync(obj)
                .ConfigureAwait(false);

        public async Task InsertOrUpdateAsync(T obj) =>
            await _conn
                .InsertOrReplaceAsync(obj)
                .ConfigureAwait(false);

        public async Task UpdateAsync(T obj) =>
            await _conn
                .UpdateAsync(obj)
                .ConfigureAwait(false);

        public async Task DeleteAsync(T obj) =>
            await _conn
                .DeleteAsync(obj)
                .ConfigureAwait(false);
    }

}
