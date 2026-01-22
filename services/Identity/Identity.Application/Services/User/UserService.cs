using Bondy.SharedKernel.Application.Abstractions.Security;
using Bondy.SharedKernel.Application.Base;
using Bondy.SharedKernel.Domain.Abstractions;
using Bondy.SharedKernel.Domain.Common;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Results.Users;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Services.User;

public class UserService : ApplicationServiceBase, IUserService
{
    private readonly IUserRepository _users;
    private readonly ICurrentUser _currentUser;

    public UserService(ILogger<UserService> logger, 
        IClock clock, 
        IUserRepository users, ICurrentUser currentUser) : base(logger, clock)
    {
        _users = users;
        _currentUser = currentUser;
    }

    public async Task<Result<Domain.Entities.User?>> GetProfile()
    {
        var user = await _users.GetByIdAsync(_currentUser.UserId);

        return Result.Success(user);
    }

    public async Task<Result> UploadAvatar()
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UpdateProfile()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<UserBasicResult>> GetBasicProfile(long userId)
    {
        var userBasic = await _users.GetBasicProfileByIdAsync(userId);

        if (userBasic == null)
            return Result.Failure<UserBasicResult>(Error.NotFound(ErrorCodes.Common.NotFound, "User not found"));

        return Result.Success(userBasic);
    }

    public async Task<Result<List<UserBasicResult>>> GetBasicProfiles(IReadOnlyCollection<long> userIds)
    {
        var userBasics = await _users.GetBasicProfilesByIdsAsync(userIds);

        return Result.Success(userBasics);
    }
}
