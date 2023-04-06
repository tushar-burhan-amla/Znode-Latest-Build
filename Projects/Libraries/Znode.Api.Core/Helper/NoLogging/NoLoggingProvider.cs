using Hangfire.Logging;

namespace Znode.Api.Core
{
    public class NoLoggingProvider : ILogProvider
    {
        public ILog GetLogger(string name)
        {
            return new NoLoggingLogger();
        }
    }
    
}
