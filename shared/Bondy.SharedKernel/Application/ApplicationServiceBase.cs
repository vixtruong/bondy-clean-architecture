

using Bondy.SharedKernel.Abstractions;
using Microsoft.Extensions.Logging;

namespace Bondy.SharedKernel.Application
{
    public class ApplicationServiceBase
    {
        protected readonly ILogger _logger;
        protected readonly IClock _clock;

        public ApplicationServiceBase(ILogger logger, IClock clock)
        {
            _logger = logger;
            _clock = clock;
        }
    }
}
