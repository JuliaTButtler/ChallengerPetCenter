namespace ChallengerPetCenter.Common
{
    public static class PagingHelper
    {
        public static (IReadOnlyList<T> Items, int Page, int PageSize, long Total) Page<T>(
            IEnumerable<T> source,
            int page,
            int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

            var list = source as IList<T> ?? source.ToList();
            var total = list.Count;
            var items = list
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, page, pageSize, total);
        }

        public static IOrderedEnumerable<T> ApplySort<T, TKey>(
            IEnumerable<T> source,
            Func<T, TKey> keySelector,
            string? order)
        {
            var descending = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
            return descending
                ? source.OrderByDescending(keySelector)
                : source.OrderBy(keySelector);
        }
    }
}
