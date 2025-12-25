using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddAsync(RefreshToken token); 
    Task<int> RevokeTokens(long userId, DateTime utcNow);
    Task<List<RefreshToken>> GetActiveTokensByUserId(long userId, DateTime now);
}