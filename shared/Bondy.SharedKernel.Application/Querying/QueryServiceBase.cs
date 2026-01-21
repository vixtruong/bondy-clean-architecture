namespace Bondy.SharedKernel.Application.Querying;

public abstract class QueryServiceBase<TEntity>
{
    protected abstract IReadOnlyDictionary<string, Func<IQueryable<TEntity>, bool, IQueryable<TEntity>>> SortMap { get; }
    protected abstract IQueryable<TEntity> DefaultSort(IQueryable<TEntity> q);

    protected IQueryable<TEntity> ApplyCommon(IQueryable<TEntity> q, PagedRequest req)
        => q.ApplySort(req.Sort, SortMap, DefaultSort);
}