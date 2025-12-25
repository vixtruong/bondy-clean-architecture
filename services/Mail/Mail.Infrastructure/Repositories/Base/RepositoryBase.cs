using Mail.Application.Abstractions.Persistence;

namespace Mail.Infrastructure.Repositories.Base;

public  class RepositoryBase
{
    protected readonly IMailDbContext _db;

    public RepositoryBase(IMailDbContext db)
    {
        _db = db;
    }
}
