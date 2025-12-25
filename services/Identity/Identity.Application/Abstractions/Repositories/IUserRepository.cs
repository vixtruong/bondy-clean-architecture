using Bondy.SharedKernel.Querying;
using Identity.Contracts.Users;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<int> UpdateAvatarUrlByIdAsync(long id, string? avatarUrl);

    Task<User?> GetByEmailAsync(Email email);

    Task<List<UserBasicResponse>> GetBasicProfilesByIdsAsync(IReadOnlyCollection<long> userIds);

    Task<UserBasicResponse?> GetBasicProfileByIdAsync(long userId);

    Task<List<User>> SearchByEmailContainsAsync(string emailPart);

    Task<PagedResult<UserBasicResponse>> GetAllBasicProfilesAsync(int pageNumber, int pageSize);
}