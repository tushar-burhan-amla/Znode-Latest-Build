using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Hangfire;
using Hangfire.SqlServer;
using Znode.Api.Core;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api
{
    public partial class Startup
    {
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            GlobalConfiguration.Configuration
                .UseLogProvider(new NoLoggingProvider())
                .UseSqlServerStorage(ConfigurationManager.ConnectionStrings["ZnodeHangfireDB"].ConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(ZnodeHangfireSettings.HangfireCommandBatchMaxTimeout),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(ZnodeHangfireSettings.HangfireSlidingInvisibilityTimeout),
                    QueuePollInterval = TimeSpan.FromMilliseconds(ZnodeHangfireSettings.HangfireQueuePollInterval),
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    PrepareSchemaIfNecessary = ZnodeHangfireSettings.HangfirePrepareSchemaIfNecessary,
                });

            yield return new BackgroundJobServer(
                new BackgroundJobServerOptions { ServerName = $"{Environment.MachineName}:{Process.GetCurrentProcess().Id}:{AppDomain.CurrentDomain.Id}" });
        }
    }
}