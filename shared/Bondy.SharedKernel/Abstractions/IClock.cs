namespace Bondy.SharedKernel.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
    DateTimeOffset UtcNowOffset { get; }

    // helper
    DateTime UtcDaysFromNow(int days);
    DateTimeOffset UtcDaysFromNowOffset(int days);

    DateTime UtcMinutesFromNow(int minutes);
    DateTimeOffset UtcMinutesFromNowOffset(int minutes);
}