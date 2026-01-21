namespace Identity.Contracts.Users;

public sealed record UserBasicResponse(
    long Id,
    string DisplayName,
    string? AvatarUrl
);