using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Repositories;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

internal class OtpCodeRepository : RepositoryBase, IOtpCodeRepository
{
    public OtpCodeRepository(IIdentityDbContext db) : base(db)
    {
    }

    public async Task<OtpCode> AddAsync(OtpCode otp)
    {
        _db.OtpCodes.Add(otp);
        await _db.SaveChangesAsync();

        return otp;
    }

    public async Task DeactivateActiveOtp(
        long subjectId,
        OtpPurpose purpose,
        DateTime now)
    {
        await _db.OtpCodes
            .Where(o =>
                o.Active &&
                o.SubjectId == subjectId &&
                o.Purpose == purpose)
            .ExecuteUpdateAsync(s => s
                .SetProperty(o => o.Active, false));
    }

    public async Task<OtpCode?> GetActiveBySubject(long subjectId, OtpPurpose purpose)
    {
        return await _db.OtpCodes.FirstOrDefaultAsync(o => o.Active && o.SubjectId == subjectId && o.Purpose == purpose);
    }

    public async Task<OtpCode> UpdateAsync(OtpCode otp)
    {
        _db.OtpCodes.Update(otp);
        await _db.SaveChangesAsync();

        return otp;
    }
}
