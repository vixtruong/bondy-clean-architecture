

using Bondy.SharedKernel.Abstractions;
using Bondy.SharedKernel.Configuration;
using Microsoft.Extensions.Logging;

namespace Bondy.SharedKernel.Application
{
    public class ApplicationServiceBase
    {
        protected readonly ILogger _logger;
        protected readonly IClock _clock;
        protected readonly AppConfigOptions _appConfigs;

        public ApplicationServiceBase(ILogger logger, IClock clock, AppConfigOptions appConfigs)
        {
            _logger = logger;
            _clock = clock;
            _appConfigs = appConfigs;
        }
    }
}
