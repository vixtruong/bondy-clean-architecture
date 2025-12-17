namespace Identity.Contracts.Auth;

public sealed record LoginRequest(string Email, string Password);

public sealed record LoginResponse(string AccessToken, string RefreshToken);

