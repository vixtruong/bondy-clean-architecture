using Bondy.SharedKernel.Domain.Abstractions;

namespace Bondy.SharedKernel.Infrastructure.Common.Clock;

public sealed class SystemClock : IClock
{
    public DateTime Now => DateTime.UtcNow;
    public DateTimeOffset NowOffset => DateTimeOffset.UtcNow;
    public DateTime DaysFromNow(int days) => DateTime.UtcNow.AddDays(days);
    public DateTimeOffset DaysFromNowOffset(int days) => DateTimeOffset.UtcNow.AddDays(days);
    public DateTime MinutesFromNow(int minutes) => DateTime.UtcNow.AddMinutes(minutes);
    public DateTimeOffset MinutesFromNowOffset(int minutes) => DateTimeOffset.UtcNow.AddMinutes(minutes);
    
}