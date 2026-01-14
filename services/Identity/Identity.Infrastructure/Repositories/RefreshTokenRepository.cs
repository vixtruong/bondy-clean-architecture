
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Repositories;
using Identity.Domain.Entities;
using Identity.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;
public sealed class RefreshTokenRepository : RepositoryBase, IRefreshTokenRepository
{
    public RefreshTokenRepository(IIdentityDbContext db) : base(db)
    {
    }

    public async Task<RefreshToken> AddAsync(RefreshToken token)
    {
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync();

        return token;
    }

    public async Task<int> RevokeTokens(long userId, string sessionId, DateTime now)
    {
        return await _db.RefreshTokens
            .Where(t => t.UserId == userId
                        && t.SessionId == sessionId
                        && !t.Revoked
                        && t.ExpiresAt > now)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.RevokedAt, now)
                .SetProperty(t => t.Revoked, true));
    }

    public async Task<List<RefreshToken>> GetActiveTokensByUserIdAndSessionId(long userId, string sessionId, DateTime now)
    {
        return await _db.RefreshTokens
            .Where(r => r.UserId == userId && r.SessionId == sessionId && r.RevokedAt == null && !r.Revoked && r.ExpiresAt > now)
            .Include(r => r.User)
            .ToListAsync();
    }
}
