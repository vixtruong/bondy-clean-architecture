using Bondy.SharedKernel.Querying;
using Identity.Contracts.Users;
using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<int> UpdateAvatarUrlByIdAsync(long id, string? avatarUrl, CancellationToken ct);

    Task<User?> GetByEmailAsync(string emailNormalized, CancellationToken ct);

    Task<List<UserBasicResponse>> GetBasicProfilesByIdsAsync(IReadOnlyCollection<long> userIds, CancellationToken ct);

    Task<UserBasicResponse?> GetBasicProfileByIdAsync(long userId, CancellationToken ct);

    Task<List<User>> SearchByEmailContainsAsync(string emailPart, CancellationToken ct);

    Task<PagedResult<UserBasicResponse>> GetAllBasicProfilesAsync(int pageNumber, int pageSize, CancellationToken ct);
}