using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Security
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(User user);
    }
}
