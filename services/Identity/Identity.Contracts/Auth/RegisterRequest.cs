using System.ComponentModel.DataAnnotations;

namespace Identity.Contracts.Auth;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string FirstName { get; set; } = default!;

    public string? MiddleName { get; set; }

    [Required]
    public string LastName { get; set; } = default!;

    [Required]
    [Timestamp]
    public DateTime Dob { get; set; } = default!;

    [Required]
    [Length(8, 24, ErrorMessage = "Password must be with a minimum length of '8' and maximum length of '24'.")]
    public string Password { get; set; } = default!;
}
