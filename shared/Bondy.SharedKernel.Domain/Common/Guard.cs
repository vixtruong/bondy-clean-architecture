using System.Globalization;

namespace Bondy.SharedKernel.Application.Common;

public static class Guard
{
    public static T NotNull<T>(T? value, string paramName) where T : class
        => value ?? throw new ArgumentNullException(paramName);

    public static string NotNullOrWhiteSpace(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        return value;
    }

    public static IReadOnlyCollection<T> NotNullOrEmpty<T>(IReadOnlyCollection<T>? value, string paramName)
    {
        if (value is null || value.Count == 0)
            throw new ArgumentException("Collection cannot be null or empty.", paramName);
        return value;
    }

    public static int Positive(int value, string paramName)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(paramName, "Value must be > 0.");
        return value;
    }

    public static long Positive(long value, string paramName)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(paramName, "Value must be > 0.");
        return value;
    }

    public static decimal NonNegative(decimal value, string paramName)
    {
        if (value < 0m) throw new ArgumentOutOfRangeException(paramName, "Value must be >= 0.");
        return value;
    }

    public static DateTime NotDefault(DateTime value, string paramName)
    {
        if (value == default) throw new ArgumentException("DateTime cannot be default.", paramName);
        return value;
    }

    public static Guid NotEmpty(Guid value, string paramName)
    {
        if (value == Guid.Empty) throw new ArgumentException("Guid cannot be empty.", paramName);
        return value;
    }

    public static string MaxLength(string value, int maxLen, string paramName)
    {
        NotNull(value, paramName);
        if (value.Length > maxLen)
            throw new ArgumentException(
                string.Format(CultureInfo.InvariantCulture, "Value length cannot exceed {0}.", maxLen),
                paramName);
        return value;
    }
}