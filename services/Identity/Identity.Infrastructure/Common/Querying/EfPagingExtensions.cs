using Bondy.SharedKernel.Application.Querying;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Common.Querying;

public static class EfPagingExtensions
{
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> q,
        int page,
        int size,
        Expression<Func<TEntity, TDto>> selector)
    {
        var total = await q.LongCountAsync();
        var items = await q
            .Skip((page - 1) * size)
            .Take(size)
            .Select(selector)
            .ToListAsync();

        return new PagedResult<TDto>(items, page, size, total);
    }
}