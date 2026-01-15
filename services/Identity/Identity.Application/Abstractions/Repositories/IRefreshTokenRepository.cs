using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddAsync(RefreshToken token); 
    Task<int> RevokeTokens(long userId, string sessionId, DateTime utcNow);
    Task<List<RefreshToken>> GetActiveTokensByUserIdAndSessionId(long userId, string sessionId, DateTime now);
}