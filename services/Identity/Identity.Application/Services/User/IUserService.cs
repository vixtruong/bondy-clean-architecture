using Bondy.SharedKernel.Domain.Common;
using Identity.Application.Results.Users;

namespace Identity.Application.Services.User;

public interface IUserService
{
    Task<Result<Domain.Entities.User?>> GetProfile();

    Task<Result> UploadAvatar();

    Task<Result> UpdateProfile();

    Task<Result<UserBasicResult>> GetBasicProfile(long userId);

    Task<Result<List<UserBasicResult>>> GetBasicProfiles(IReadOnlyCollection<long> userIds);
}
