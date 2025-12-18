using Bondy.SharedKernel.Abstractions;

namespace Identity.Infrastructure.Common.Clock
{
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
        public DateTime UtcDaysFromNow(int days) => DateTime.UtcNow.AddDays(days);
        public DateTimeOffset UtcDaysFromNowOffset(int days) => DateTimeOffset.UtcNow.AddDays(days);
        public DateTime UtcMinutesFromNow(int minutes) => DateTime.UtcNow.AddMinutes(minutes);
        public DateTimeOffset UtcMinutesFromNowOffset(int minutes) => DateTimeOffset.UtcNow.AddMinutes(minutes);
        
    }
}
