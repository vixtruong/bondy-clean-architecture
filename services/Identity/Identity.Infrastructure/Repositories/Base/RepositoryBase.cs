using Identity.Application.Abstractions.Persistence;

namespace Identity.Infrastructure.Repositories.Base;
public abstract class RepositoryBase
{
    protected readonly IIdentityDbContext _db;

    protected RepositoryBase(IIdentityDbContext db)
    {
        _db = db;
    }
}
