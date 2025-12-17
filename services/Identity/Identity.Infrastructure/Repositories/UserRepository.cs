using Identity.Application.Abstractions.Repositories;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _db;

    public UserRepository(IdentityDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByIdAsync(long id, CancellationToken ct)
        => _db.Users
            .Include(x => x.Accounts)
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<User?> GetByEmailAsync(string emailNormalized, CancellationToken ct)
        => _db.Users
            .FirstOrDefaultAsync(x => x.Email.Value == emailNormalized, ct);

    public Task AddAsync(User user, CancellationToken ct)
        => _db.Users.AddAsync(user, ct).AsTask();
}