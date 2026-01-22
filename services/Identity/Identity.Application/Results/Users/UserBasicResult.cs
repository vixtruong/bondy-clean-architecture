
namespace Identity.Application.Results.Users;

public sealed record UserBasicResult(
    long Id,
    string DisplayName,
    string? AvatarUrl
);