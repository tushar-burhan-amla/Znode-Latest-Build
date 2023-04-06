using System;
using System.Configuration;

namespace Znode.Libraries.Caching
{
    public class CacheFrameworkSettings
    {
        private static readonly string CacheEventsIndexNameSuffix = "events";
        private static readonly string CacheMetricsIndexNameSuffix = "metrics";

        public static string GetCacheEventIndexName()
        {
            return $"{GetIndexBaseName()}{CacheEventsIndexNameSuffix}";
        }

        public static string GetCacheMetricsIndexName()
        {
            return $"{GetIndexBaseName()}{CacheMetricsIndexNameSuffix}";
        }

        public static bool IsCacheMetricRecordingEnabled()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["CacheMetricRecordingEnabled"]);
        }

        public static int GetCachePollFrequencyInMilliseconds()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["CachePollFrequencyInMilliseconds"]);
        }

        private static string GetIndexBaseName()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["CacheIndexBaseName"]).ToLower();
        }
    }
}
