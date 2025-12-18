using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddAsync(RefreshToken token); 
    Task<int> RevokeTokens(long userId, DateTime utcNow);
    Task<bool> IsValidToken(long userId, string tokenRaw, DateTime utcNow);
}