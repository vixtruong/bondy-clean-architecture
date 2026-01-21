using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bondy.SharedKernel.Domain.Abstractions;
using Identity.Application.Abstractions.Security;
using Identity.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Common.Security;

public sealed class TokenGenerator : ITokenGenerator
{
    private readonly JwtOptions _opt;
    private readonly IClock _clock;

    public TokenGenerator(IOptions<JwtOptions> opt, IClock clock)
    {
        _clock = clock;
        _opt = opt.Value;
    }

    public (string AccessToken, int AccessTokenMinutes) GenerateAccessToken(User user)
    {
        var now = _clock.Now;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Code));
        }

        foreach (var scope in user.GetEffectiveScopes())
        {
            claims.Add(new Claim("scope", scope));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_opt.AccessTokenMinutes),
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), _opt.AccessTokenMinutes);
    }
}