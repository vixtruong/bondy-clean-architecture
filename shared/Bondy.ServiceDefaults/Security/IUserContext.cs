
namespace Bondy.ServiceDefaults.Security;

public interface IUserContext
{
    string AuthType { get; }
    string IdentityId { get; }
    string Owner { get; }
    IReadOnlyCollection<string> Scopes { get; }
    string? Role { get; }
}