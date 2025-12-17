using System.Linq.Expressions;
using Bondy.SharedKernel.Querying;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Querying;

public static class EfPagingExtensions
{
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> q,
        int page,
        int size,
        Expression<Func<TEntity, TDto>> selector,
        CancellationToken ct)
    {
        var total = await q.LongCountAsync(ct);
        var items = await q
            .Skip((page - 1) * size)
            .Take(size)
            .Select(selector)
            .ToListAsync(ct);

        return new PagedResult<TDto>(items, page, size, total);
    }
}