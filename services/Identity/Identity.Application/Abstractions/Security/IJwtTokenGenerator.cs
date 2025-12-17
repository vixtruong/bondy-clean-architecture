using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Security
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(User user);
    }
}
