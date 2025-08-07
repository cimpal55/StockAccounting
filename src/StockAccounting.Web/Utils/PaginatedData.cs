using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;

namespace StockAccounting.Web.Utils
{
    public class PaginatedData<T> : List<T>
    {
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int TotalData { get; }
        private PaginatedData(IEnumerable<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalData = count;
            AddRange(items);
        }

        public static async Task<PaginatedData<T>> CreateListAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();

            var items = await source.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedData<T>(items, count, pageIndex, pageSize);
        }

    }
}