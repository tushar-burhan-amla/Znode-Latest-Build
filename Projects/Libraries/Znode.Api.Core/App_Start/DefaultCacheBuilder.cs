using System;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api
{
    public static class DefaultCacheBuilder
    {
        private const string logComponentName = "Diagnostics";

        public static void TryBuildCaches()
        {
            TryBuildCache(ApiHelper.CacheActivePromotions, "active promotions");
            TryBuildCache(DefaultGlobalConfigSettingHelper.DefaultGlobalConfigSettingCache, "default global settings");
            TryBuildCache(DefaultGlobalConfigSettingHelper.DefaultLoggingConfigSettingCache, "default logging settings");
            ZnodeLogging.LogMessage("Caches successfully built.", logComponentName, System.Diagnostics.TraceLevel.Verbose);
        }

        private static void TryBuildCache(Action buildCache, string description)
        {
            try
            {
                ZnodeLogging.LogMessage($"About to try to build '{description}' cache.", logComponentName, System.Diagnostics.TraceLevel.Verbose);
                buildCache();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Failed to build '{description}' cache: {ex.Message}", logComponentName, System.Diagnostics.TraceLevel.Error, ex);
            }
        }
    }
}