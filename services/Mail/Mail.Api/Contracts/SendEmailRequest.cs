using Bondy.SharedKernel.Application.Commands;
using System.ComponentModel.DataAnnotations;

namespace Mail.Api.Contracts;

/// <summary>
/// Message/DTO để yêu cầu Mail service gửi email theo template.
/// </summary>
public sealed class SendEmailRequest
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