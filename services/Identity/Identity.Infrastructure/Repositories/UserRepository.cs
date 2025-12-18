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

public sealed class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(IIdentityDbContext db) : base(db)
    {
    }

    public async Task<int> UpdateAvatarUrlByIdAsync(long id, string? avatarUrl, CancellationToken ct)
    {
        return await _db.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.AvatarUrl, avatarUrl),
                ct);
    }

    public Task<User?> GetByEmailAsync(string emailNormalized, CancellationToken ct)
    {
        // Tuỳ bạn normalize thế nào; ở đây so thẳng Value
        var email = Email.Create(emailNormalized);

        return _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public Task<List<UserBasicResponse>> GetBasicProfilesByIdsAsync(IReadOnlyCollection<long> userIds, CancellationToken ct)
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
            .ToListAsync(ct);
    }

    public Task<UserBasicResponse?> GetBasicProfileByIdAsync(long userId, CancellationToken ct)
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
            .FirstOrDefaultAsync(ct);
    }

    public Task<List<User>> SearchByEmailContainsAsync(string emailPart, CancellationToken ct)
    {
        var term = (emailPart ?? "").Trim();
        if (term.Length == 0) return Task.FromResult(new List<User>());

        return _db.Users
            .AsNoTracking()
            .Where(u => EF.Functions.ILike(u.Email.Value, $"%{term}%"))
            .ToListAsync(ct);
    }

    public Task<PagedResult<UserBasicResponse>> GetAllBasicProfilesAsync(int pageNumber, int pageSize, CancellationToken ct)
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
            ),
            ct);
    }
}
