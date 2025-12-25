using Bondy.SharedKernel.Abstractions;

namespace Mail.Infrastructure.Common.Clock
{
    public sealed class SystemClock : IClock
    {
        public DateTime Now => DateTime.Now;
        public DateTimeOffset NowOffset => DateTimeOffset.Now;
        public DateTime DaysFromNow(int days) => DateTime.Now.AddDays(days);
        public DateTimeOffset DaysFromNowOffset(int days) => DateTimeOffset.Now.AddDays(days);
        public DateTime MinutesFromNow(int minutes) => DateTime.Now.AddMinutes(minutes);
        public DateTimeOffset MinutesFromNowOffset(int minutes) => DateTimeOffset.Now.AddMinutes(minutes);
        
    }
}
