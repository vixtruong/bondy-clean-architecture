using Bondy.SharedKernel.Querying;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Repositories;
using Identity.Contracts.Users;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Common.Querying;
using Identity.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public sealed class UserRepository : RepositoryBase, IUserRepository
{
    public UserRepository(IIdentityDbContext db) : base(db)
    {
    }

    public async Task<User> AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user;
    }

    public async Task<int> UpdateAvatarUrlByIdAsync(long id, string? avatarUrl)
    {
        return await _db.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.AvatarUrl, avatarUrl));
    }

    public Task<User?> GetByEmailAsync(Email email)
    {
        return _db.Users
            .AsNoTracking()
            .Include(u => u.Accounts)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task<List<UserBasicResponse>> GetBasicProfilesByIdsAsync(IReadOnlyCollection<long> userIds)
    {
        if (userIds.Count == 0) return Task.FromResult(new List<UserBasicResponse>());

        return _db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new UserBasicResponse(
                u.Id,
                u.Name.ToString(),
                u.AvatarUrl,
                u.FriendCount
            ))
            .ToListAsync();
    }

    public Task<UserBasicResponse?> GetBasicProfileByIdAsync(long userId)
    {
        return _db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserBasicResponse(
                u.Id,
                u.Name.ToString(),
                u.AvatarUrl,
                u.FriendCount
            ))
            .FirstOrDefaultAsync();
    }

    public Task<List<User>> SearchByEmailContainsAsync(string emailPart)
    {
        var term = (emailPart ?? "").Trim();
        if (term.Length == 0) return Task.FromResult(new List<User>());

        return _db.Users
            .AsNoTracking()
            .Where(u => EF.Functions.ILike(u.Email.Value, $"%{term}%"))
            .ToListAsync();
    }

    public Task<PagedResult<UserBasicResponse>> GetAllBasicProfilesAsync(int pageNumber, int pageSize)
    {
        var q = _db.Users
            .AsNoTracking()
            .OrderByDescending(u => u.Id);

        return q.ToPagedResultAsync(
            pageNumber,
            pageSize,
            u => new UserBasicResponse(
                u.Id,
                u.Name.ToString(),
                u.AvatarUrl,
                u.FriendCount
            ));
    }

    public async Task<bool> ExistByEmailAsync(Email email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }
}
