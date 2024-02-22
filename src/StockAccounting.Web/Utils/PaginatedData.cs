namespace StockAccounting.Web.Utils
{
    public class PaginatedData<T> : List<T>
    {
        public int PageIndex { get; }

        public int TotalPages { get; }

        public int TotalData { get; }

        public PaginatedData(IEnumerable<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalData = count;
            AddRange(items);
        }

        public static PaginatedData<T> CreateList(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var enumerable = source.ToList();
            var count = enumerable.Count();

            var items = enumerable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedData<T>(items, count, pageIndex, pageSize);
        }
    }
}
