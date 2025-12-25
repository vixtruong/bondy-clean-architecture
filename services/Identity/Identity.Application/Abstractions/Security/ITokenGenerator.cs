using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Security
{
    public interface ITokenGenerator
    {
        (string AccessToken, int AccessTokenMinutes) GenerateAccessToken(User user);
    }
}
