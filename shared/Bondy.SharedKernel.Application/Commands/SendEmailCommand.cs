
namespace Bondy.SharedKernel.Application.Commands;

public sealed class SendEmailCommand
{
    public string To { get; set; } = default!;

    public EmailPurpose Purpose { get; set; }

    public Dictionary<string, string> Data { get; set; } = new();

    public string? DedupTokenId { get; set; }
}

public enum EmailPurpose
{
    Welcome = 1,
    OAuth2Welcome = 2,
    Registration = 3,
    ResetPassword = 4
}