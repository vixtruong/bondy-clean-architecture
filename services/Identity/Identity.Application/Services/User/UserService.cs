using Bondy.SharedKernel.Application.Abstractions.Security;
using Bondy.SharedKernel.Application.Base;
using Bondy.SharedKernel.Domain.Abstractions;
using Bondy.SharedKernel.Domain.Common;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Results.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Storage.Abstractions;

namespace Identity.Application.Services.User;

public class UserService : ApplicationServiceBase, IUserService
{
    private readonly IUserRepository _users;
    private readonly ICurrentUser _currentUser;
    private readonly IFileStorage _fileStorage;

    public UserService(ILogger<UserService> logger, 
        IClock clock, 
        IUserRepository users, ICurrentUser currentUser, IFileStorage fileStorage) : base(logger, clock)
    {
        _users = users;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task<Result<Domain.Entities.User?>> GetProfile()
    {
        var user = await _users.GetByIdAsync(_currentUser.UserId);

        return Result.Success(user);
    }

    public async Task<Result> UploadAvatar(IFormFile file)
    {
        if (file.Length == 0)
            return Result.Failure(Error.BadRequest(ErrorCodes.Validation.Required, "No file uploaded"));

        var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        const long maxBytes = 5 * 1024 * 1024;

        if (file.Length > maxBytes)
            return Result.Failure(Error.BadRequest(ErrorCodes.Validation.Argument, "File too large. Max 5 MB."));

        var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? string.Empty;
        if (!allowedExt.Contains(ext))
            return Result.Failure(Error.BadRequest(ErrorCodes.Validation.Argument, "Invalid file type. Allowed: jpg, jpeg, png, webp."));

        var user = await _users.GetByIdAsync(_currentUser.UserId);
        if (user == null)
            return Result.Failure(Error.NotFound(ErrorCodes.Common.NotFound, "User not found."));

        var fileName = $"{Guid.NewGuid()}{ext}";
        var objectPath = $"users/{user.Id}/avatar/{fileName}";

        try
        {
            await using var stream = file.OpenReadStream();
            var uploadedUrl = await _fileStorage.UploadAsync(stream, objectPath, file.ContentType);

            user.SetAvatarUrl(uploadedUrl);
            
            await _users.UpdateAsync(user);


            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Error.BadRequest(ErrorCodes.Common.Unknown, "Upload cancelled"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload avatar for user {UserId}", _currentUser.UserId);
            return Result.Failure(Error.InternalServer(ErrorCodes.Common.Unknown, "Failed to upload avatar"));
        }
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
