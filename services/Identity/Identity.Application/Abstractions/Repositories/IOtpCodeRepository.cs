using Identity.Domain.Entities;
using Identity.Domain.Enums;

namespace Identity.Application.Abstractions.Repositories;

public interface IOtpCodeRepository
{
    Task<OtpCode> AddAsync(OtpCode otp);
    Task DeactivateActiveOtp(long subjectId, OtpPurpose purpose, DateTime now);
    Task<OtpCode?> GetActiveBySubject(long subjectId, OtpPurpose purpose);
    Task<OtpCode> UpdateAsync(OtpCode otp);
}
