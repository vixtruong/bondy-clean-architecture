
namespace Bondy.SharedKernel.Application.Querying
{
    // Bondy.SharedKernel.Querying
    public static class QueryablePagingExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> q, int page, int size)
            => q.Skip((page - 1) * size).Take(size);
    }

}
