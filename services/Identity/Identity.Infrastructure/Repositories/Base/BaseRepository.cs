using Bondy.SharedKernel.Abstractions;
using Identity.Application.Abstractions.Persistence;

namespace Identity.Infrastructure.Repositories.Base;
public abstract class BaseRepository
{
    protected readonly IIdentityDbContext _db;

    protected BaseRepository(IIdentityDbContext db)
    {
        _db = db;
    }
}
