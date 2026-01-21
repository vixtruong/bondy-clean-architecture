using Identity.Application.Abstractions.Security;
using Identity.Domain.Entities;
using System.Security.Cryptography;

namespace Identity.Infrastructure.Common.Security;

public class TempPasswordGenerator : ITempPasswordGenerator
{
    private const int Length = 32;

    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(Length);
        return Convert.ToBase64String(bytes);
    }
}
