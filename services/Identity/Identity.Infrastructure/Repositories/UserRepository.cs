using Bondy.SharedKernel.Application.Querying;
using Bondy.SharedKernel.Infrastructure.Common.Querying;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Results.Users;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
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

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
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
            .AsSplitQuery()
            .AsNoTracking()
            .Include(u => u.Accounts)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _db.Users.FindAsync(id);
    }

    public Task<User?> GetByIdForTokenAsync(long userId)
    {
        return _db.Users
            .AsSplitQuery()
            .Include(u => u.Roles)
                .ThenInclude(r => r.Scopes)
            .Include(u => u.GrantedScopes)
            .Include(u => u.DeniedScopes)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public Task<List<UserBasicResult>> GetBasicProfilesByIdsAsync(IReadOnlyCollection<long> userIds)
    {
        if (userIds.Count == 0) return Task.FromResult(new List<UserBasicResult>());

        return _db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new UserBasicResult(
                u.Id,
                u.Name.ToString(),
                u.AvatarUrl
                ))
            .ToListAsync();
    }

    public Task<UserBasicResult?> GetBasicProfileByIdAsync(long userId)
    {
        return _db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserBasicResult(
                u.Id,
                u.Name.ToString(),
                u.AvatarUrl
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

    public Task<PagedResult<UserBasicResult>> GetAllBasicProfilesAsync(int pageNumber, int pageSize)
    {
        var q = _db.Users
            .AsNoTracking()
            .OrderByDescending(u => u.Id);

        return q.ToPagedResultAsync(
            pageNumber,
            pageSize,
            u => new UserBasicResult(
                u.Id,
                u.Name.ToString(),
                u.AvatarUrl
            ));
    }

    public async Task<bool> ExistByEmailAsync(Email email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }
}
