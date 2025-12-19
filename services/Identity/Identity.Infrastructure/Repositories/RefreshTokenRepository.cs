
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Abstractions.Security;
using Identity.Domain.Entities;
using Identity.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;
public sealed class RefreshTokenRepository : RepositoryBase, IRefreshTokenRepository
{
    private readonly IHasher _hasher;

    public RefreshTokenRepository(IIdentityDbContext db, IHasher hasher) : base(db)
    {
        _hasher = hasher;
    }

    public async Task<RefreshToken> AddAsync(RefreshToken token)
    {
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync();

        return token;
    }

    public async Task<int> RevokeTokens(long userId, DateTime utcNow)
    {
        return await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.IsActive(utcNow))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.RevokedAt, utcNow)
                .SetProperty(t => t.Revoked, true)
                .SetProperty(t => t.UpdatedAt, utcNow));
    }

    public async Task<bool> IsValidToken(long userId, string tokenRaw, DateTime utcNow)
    {
        return await _db.RefreshTokens
            .AsNoTracking()
            .AnyAsync(t =>
                t.UserId == userId
                && _hasher.Verify(tokenRaw, t.TokenHash.Value)
                && !t.IsExpired(utcNow));
    }
}
