using Bondy.SharedKernel.Application.Querying;
using System.Linq.Expressions;
using Identity.Application.Abstractions.Persistence;
using Identity.Contracts.Users;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Common.Querying;

public sealed class UserQueryService : QueryServiceBase<Domain.Entities.User>
{
    private readonly IIdentityDbContext _db;

    public UserQueryService(IIdentityDbContext db) => _db = db;

    protected override IReadOnlyDictionary<string, Func<IQueryable<Domain.Entities.User>, bool, IQueryable<Domain.Entities.User>>> SortMap
        => new Dictionary<string, Func<IQueryable<Domain.Entities.User>, bool, IQueryable<Domain.Entities.User>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["createdAt"] = (q, desc) => desc ? q.OrderByDescending(x => x.CreatedAt) : q.OrderBy(x => x.CreatedAt),
            ["email"] = (q, desc) => desc ? q.OrderByDescending(x => x.Email.Value) : q.OrderBy(x => x.Email.Value),
            ["friendCount"] = (q, desc) => desc ? q.OrderByDescending(x => x.FriendCount) : q.OrderBy(x => x.FriendCount),
        };

    protected override IQueryable<Domain.Entities.User> DefaultSort(IQueryable<Domain.Entities.User> q)
        => q.OrderByDescending(x => x.Id);

    private static readonly Expression<Func<Domain.Entities.User, UserBasicResponse>> BasicSelector =
        u => new UserBasicResponse(
            u.Id,
            // Nếu bạn muốn join name “đẹp” như code cũ, nên precompute ở DB (FullName) hoặc map ra app layer.
            // Ở đây giữ tối giản để EF dịch chắc chắn:
            (u.Name.FirstName ?? "") + " " + (u.Name.MiddleName ?? "") + " " + (u.Name.LastName ?? ""),
            u.AvatarUrl,
            u.FriendCount
        );

    public Task<PagedResult<UserBasicResponse>> SearchAsync(UserListRequest req)
    {
        IQueryable<Domain.Entities.User> q = _db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(req.EmailContains))
        {
            var term = req.EmailContains.Trim();
            q = q.Where(u => EF.Functions.ILike(u.Email.Value, $"%{term}%"));
        }

        if (req.Active.HasValue)
            q = q.Where(u => u.Active == req.Active.Value);

        q = ApplyCommon(q, req);

        return q.ToPagedResultAsync(req.PageNumber, req.PageSize, BasicSelector);
    }
}
