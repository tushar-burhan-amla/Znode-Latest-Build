using Hangfire.Logging;
using System;

namespace Znode.Api.Core
{
    public class NoLoggingLogger : ILog
    {
        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            return logLevel > LogLevel.Info;
        }
    }
}
