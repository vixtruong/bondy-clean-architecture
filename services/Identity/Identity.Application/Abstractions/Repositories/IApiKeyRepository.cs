using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Repositories;

public interface IApiKeyRepository
{
    // ===== Authentication =====

    /// <summary>
    /// Lookup API key by public prefix (fast path).
    /// </summary>
    Task<ApiKey?> GetByKeyPrefitAsync(string keyPrefit);


    // ===== Management =====

    /// <summary>
    /// Get API key by id (admin / management).
    /// </summary>
    Task<ApiKey?> GetByIdAsync(long id);

    /// <summary>
    /// Get all API keys of an owner.
    /// </summary>
    Task<IReadOnlyList<ApiKey>> GetByOwnerAsync(string owner);

    /// <summary>
    /// Check prefix uniqueness before creation.
    /// </summary>
    Task<bool> ExistsByKeyPrefitAsync(string keyPrefit, DateTime now);


    // ===== Persistence =====

    Task AddAsync(ApiKey apiKey);

    Task<int> UpdateAsync(ApiKey apiKey);

    Task<int> RemoveAsync(ApiKey apiKey);


    // ===== Revocation =====

    /// <summary>
    /// Revoke a single API key.
    /// </summary>
    Task<int> RevokeAsync(ApiKey apiKey);

    /// <summary>
    /// Revoke all API keys of an owner.
    /// </summary>
    Task<int> RevokeAllByOwnerAsync(string owner);


    // ===== Validation / Housekeeping =====

    /// <summary>
    /// Get active, non-expired API keys.
    /// </summary>
    Task<IReadOnlyList<ApiKey>> GetActiveAsync();

    /// <summary>
    /// Remove expired API keys (background job).
    /// </summary>
    Task<int> RemoveExpiredAsync(DateTimeOffset now);


    // ===== Audit =====

    /// <summary>
    /// Update last used timestamp.
    /// </summary>
    Task<int> TouchAsync(ApiKey apiKey);
}