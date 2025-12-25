using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Repositories;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

internal class PreRegistrationRepository : RepositoryBase, IPreRegistrationRepository
{
    public PreRegistrationRepository(IIdentityDbContext db) : base(db)
    {
    }

    public async Task<PreRegistration> AddAsync(PreRegistration pre)
    {
        _db.PreRegistrations.Add(pre);
        await _db.SaveChangesAsync();

        return pre;
    }

    public async Task<PreRegistration?> GetByEmailAsync(Email email)
    {
        return await _db.PreRegistrations.FirstOrDefaultAsync(p => p.Email == email);
    }

    public async Task RemoveAsync(PreRegistration pre)
    {
        _db.PreRegistrations.Remove(pre);
        await _db.SaveChangesAsync();
    }
}
