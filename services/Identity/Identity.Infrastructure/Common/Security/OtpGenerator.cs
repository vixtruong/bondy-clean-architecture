using Identity.Application.Abstractions.Security;
using System.Security.Cryptography;

namespace Identity.Infrastructure.Common.Security;

public sealed class OtpGenerator : IOtpGenerator
{
    public string Generate(int length = 6)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

        // OTP digits [0-9]
        Span<char> chars = stackalloc char[length];
        for (int i = 0; i < length; i++)
        {
            var digit = RandomNumberGenerator.GetInt32(0, 10);
            chars[i] = (char)('0' + digit);
        }
        return new string(chars);
    }
}
