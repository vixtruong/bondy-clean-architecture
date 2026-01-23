
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Repositories;
using Identity.Domain.Entities;
using Identity.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class ApiKeyRepository : RepositoryBase, IApiKeyRepository
{
    public ApiKeyRepository(IIdentityDbContext db) : base(db)
    {
    }

    public async Task<ApiKey?> GetByKeyPrefitAsync(string keyPrefit)
    {
        return await _db.ApiKeys.FirstOrDefaultAsync(k => k.KeyPrefix == keyPrefit);
    }

    public async Task<ApiKey?> GetByIdAsync(long id)
    {
        return await _db.ApiKeys.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<IReadOnlyList<ApiKey>> GetByOwnerAsync(string owner)
    {
        return await _db.ApiKeys.Where(k => k.Owner == owner).ToListAsync();
    }

    public async Task<bool> ExistsByKeyPrefitAsync(string keyPrefix, DateTime now)
    {
        return await _db.ApiKeys.AnyAsync(k => 
            k.KeyPrefix == keyPrefix &&
            k.IsActive &&
            (k.ExpiresAt == null || k.ExpiresAt > now));
    }

    public async Task AddAsync(ApiKey apiKey)
    {
        _db.ApiKeys.Add(apiKey);
        await _db.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(ApiKey apiKey)
    {
        throw new NotImplementedException();
    }

    public async Task<int> RemoveAsync(ApiKey apiKey)
    {
        throw new NotImplementedException();
    }

    public async Task<int> RevokeAsync(ApiKey apiKey)
    {
        throw new NotImplementedException();
    }

    public async Task<int> RevokeAllByOwnerAsync(string owner)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<ApiKey>> GetActiveAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<int> RemoveExpiredAsync(DateTimeOffset now)
    {
        throw new NotImplementedException();
    }

    public async Task<int> TouchAsync(ApiKey apiKey)
    {
        _db.ApiKeys.Update(apiKey);
        return await _db.SaveChangesAsync();
    }
}
