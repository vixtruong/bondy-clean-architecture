namespace Bondy.SharedKernel.Querying
{
    public static class QueryableSortExtensions
    {
        public static IQueryable<T> ApplySort<T>(
            this IQueryable<T> q,
            string? sort,
            IReadOnlyDictionary<string, Func<IQueryable<T>, bool, IQueryable<T>>> map,
            Func<IQueryable<T>, IQueryable<T>> defaultSort)
        {
            if (string.IsNullOrWhiteSpace(sort))
                return defaultSort(q);

            var s = sort.Trim();
            var desc = s.StartsWith("-", StringComparison.Ordinal);
            var key = desc ? s[1..] : s;

            if (string.IsNullOrWhiteSpace(key))
                return defaultSort(q);

            if (!map.TryGetValue(key, out var apply))
                return defaultSort(q);

            return apply(q, desc);
        }
    }
}