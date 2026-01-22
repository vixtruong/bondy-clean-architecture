using Bondy.SharedKernel.Application.Querying;
using Identity.Application.Results.Users;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);

    Task<int> UpdateAvatarUrlByIdAsync(long id, string? avatarUrl);

    Task<User?> GetByEmailAsync(Email email);
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByIdForTokenAsync(long userId);

    Task<List<UserBasicResult>> GetBasicProfilesByIdsAsync(IReadOnlyCollection<long> userIds);

    Task<UserBasicResult?> GetBasicProfileByIdAsync(long userId);

    Task<List<User>> SearchByEmailContainsAsync(string emailPart);

    Task<PagedResult<UserBasicResult>> GetAllBasicProfilesAsync(int pageNumber, int pageSize);

    Task<bool> ExistByEmailAsync(Email email);
}