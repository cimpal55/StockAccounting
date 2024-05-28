namespace StockAccounting.Core.Data.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<int> InsertAsync(T obj);
        Task InsertOrUpdateAsync(T obj);
        Task UpdateAsync(T obj);
        Task DeleteAsync(T obj);
    }
}
