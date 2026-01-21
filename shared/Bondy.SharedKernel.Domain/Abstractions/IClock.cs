
namespace Bondy.SharedKernel.Domain.Abstractions;

public interface IClock
{
    DateTime Now { get; }
    DateTimeOffset NowOffset { get; }

    // helper
    DateTime DaysFromNow(int days);
    DateTimeOffset DaysFromNowOffset(int days);

    DateTime MinutesFromNow(int minutes);
    DateTimeOffset MinutesFromNowOffset(int minutes);
}