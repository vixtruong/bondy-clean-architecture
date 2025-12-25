using Bondy.SharedKernel.Abstractions;

namespace Identity.Infrastructure.Common.Clock
{
    public sealed class SystemClock : IClock
    {
        public DateTime Now => DateTime.Now.ToUniversalTime();
        public DateTimeOffset NowOffset => DateTimeOffset.Now.ToUniversalTime();
        public DateTime DaysFromNow(int days) => DateTime.Now.ToUniversalTime().AddDays(days);
        public DateTimeOffset DaysFromNowOffset(int days) => DateTimeOffset.Now.ToUniversalTime().AddDays(days);
        public DateTime MinutesFromNow(int minutes) => DateTime.Now.ToUniversalTime().AddMinutes(minutes);
        public DateTimeOffset MinutesFromNowOffset(int minutes) => DateTimeOffset.Now.ToUniversalTime().AddMinutes(minutes);
        
    }
}
