using System.ComponentModel.DataAnnotations;

namespace Identity.Contracts.Auth;

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [Length(8, 24, ErrorMessage = "Password must be with a minimum length of '8' and maximum length of '24'.")]
    public string Password { get; set; } = default!;
}

public sealed record LoginResponse(string AccessToken, string RefreshToken);