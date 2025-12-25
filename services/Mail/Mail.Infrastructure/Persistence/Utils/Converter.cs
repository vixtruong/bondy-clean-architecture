using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mail.Infrastructure.Persistence.Utils;

public static class Converter
{
    public static readonly ValueConverter<DateTime, DateTime> UtcConverter =
        new(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );
}
