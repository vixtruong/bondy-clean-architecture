using Bondy.Contracts.Enums.Mail;
using System.ComponentModel.DataAnnotations;

namespace Bondy.Contracts.Dtos.Mail;

/// <summary>
/// Message/DTO để yêu cầu Mail service gửi email theo template.
/// </summary>
public sealed class SendEmailDto
{
    [Required(ErrorMessage = "To Email is require.")]
    [EmailAddress]
    public string To { get; set; } = default!;

    [Required(ErrorMessage = "Purpose is require.")]
    public EmailPurpose Purpose { get; set; }

    [Required]
    public Dictionary<string, string> Data { get; set; } = new();

    public string? DedupTokenId { get; set; }
}