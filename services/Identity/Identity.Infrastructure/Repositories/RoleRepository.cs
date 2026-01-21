using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Repositories;
using Identity.Domain.Entities;
using Identity.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public sealed class RoleRepository : RepositoryBase, IRoleRepository
{
    public RoleRepository(IIdentityDbContext db) : base(db)
    {
    }

    public async Task<Role?> GetByCodeAsync(string code)
    {
        return await _db.Roles
            .Include(r => r.Scopes)
            .FirstOrDefaultAsync(r => r.Code == code);
    }

    public async Task<IReadOnlyCollection<Role>> GetByCodesAsync(
        IEnumerable<string> codes)
    {
        var set = codes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        return await _db.Roles
            .Include(r => r.Scopes)
            .Where(r => set.Contains(r.Code))
            .ToListAsync();
    }

    public async Task AddAsync(Role role)
    {
        _db.Roles.Add(role);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Role role)
    {
        _db.Roles.Update(role);
        await _db.SaveChangesAsync();
    }
}