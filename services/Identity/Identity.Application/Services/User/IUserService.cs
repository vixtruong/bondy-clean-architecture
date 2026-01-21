using Bondy.SharedKernel.Domain.Common;
using Identity.Contracts.Users;

namespace Identity.Application.Services.User;

public interface IUserService
{
    Task<Result<Domain.Entities.User?>> GetProfile();

    Task<Result> UploadAvatar();

    Task<Result> UpdateProfile();

    Task<Result<UserBasicResponse>> GetBasicProfile(long userId);

    Task<Result<List<UserBasicResponse>>> GetBasicProfiles(IReadOnlyCollection<long> userIds);
}
