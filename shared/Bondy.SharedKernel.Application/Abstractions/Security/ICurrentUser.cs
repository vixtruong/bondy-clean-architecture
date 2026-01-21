namespace Bondy.SharedKernel.Application.Abstractions.Security;

public interface ICurrentUser
{
    long UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
