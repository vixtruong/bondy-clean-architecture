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
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = default!;
}
