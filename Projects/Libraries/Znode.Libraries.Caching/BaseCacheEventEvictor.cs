using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;

namespace Znode.Libraries.Caching
{

    public abstract class BaseCacheEventEvictor<T> : ICacheEventEvictor<T> where T : BaseCacheEvent, new()
    {
        /// <summary>
        /// Used for reflection. Must be kept in sync with actual name of method on this class.
        /// </summary>
        public static readonly string HandleCacheEventMethodName = "HandleCacheEvent";

        /// <summary>
        /// Used for reflection. Must be kept in sync with actual name of method on this class.
        /// </summary>
        public static readonly string GetCacheEventTypeMethodName = "GetCacheEventType";

        /// <summary>
        /// Used for reflection. Must be kept in sync with actual naming convention used to identify evictor classes.
        /// </summary>
        public static readonly string EvictorSuffix = "_Evictor";

        /// <summary>
        /// True if recording of metrics is enabled.
        /// </summary>
        private bool isCacheMetricRecordingEnabled = CacheFrameworkSettings.IsCacheMetricRecordingEnabled();

        /// <summary>
        /// Metrics recorded by this evictor during current execution. Only used if metrics enabled.
        /// </summary>
        private List<ICacheEventMetric> cacheMetrics = new List<ICacheEventMetric>();

        /// <summary>
        /// Writes metrics to elasticsearch. Only used if metrics enabled.
        /// </summary>
        private static ICacheEventMetricRecorder cacheEventMetricRecorder = new ElasticsearchCacheEventMetricRecorder();

        public BaseCacheEventEvictor()
        {

        }

        protected abstract void Setup(T cacheEvent);

        /// <summary>
        /// Directly evicts non-dictionary cached data that has become stale as a result of the
        /// provided cache event.
        /// 
        /// Override in derived class to perform necessary evictions, for example,
        /// removing cached HTML views from memory, or removing cached content in
        /// an edge service such as Cloudflare. 
        /// </summary>
        public void HandleCacheEvent(T cacheEvent)
        {

            // Allow evictor to perform handle any 1-time initialization:
            Setup(cacheEvent);

            // Tell evictor to evict non-dictionary cached data. For example, clear something from the HTML cache,
            // or clear something from Cloudflare:
            EvictNonDictionaryCacheData(cacheEvent);

            // Remove any exact keys from the dictionary cache that the evictor says are stale.
            var exactKeysToRemove = EvictSpecificDictionaryCacheKeys(cacheEvent);
            foreach (var exactKey in exactKeysToRemove)
            {
                if (HttpRuntime.Cache[exactKey] != null)
                {
                    EvictDictionaryItem(cacheEvent, exactKey);
                }
            }

            // Iterate remaining items in dictionary cache, asking evictor if each item is stale and removing it
            // if it is stale:
            IDictionaryEnumerator cacheEnumerator = HttpRuntime.Cache.GetEnumerator();
            List<string> excludedKeys = CacheItemNotToRemove();
            while (cacheEnumerator.MoveNext())
            {
                if (cacheEnumerator.Key != null)
                {
                    string currentKey = cacheEnumerator.Key.ToString();
                    bool keyExcluded = excludedKeys.Contains(currentKey);
                    bool shouldEvict = !keyExcluded && IsDictionaryItemStale(cacheEvent, currentKey);
                    if (shouldEvict)
                    {
                        EvictDictionaryItem(cacheEvent, currentKey);
                    }
                }
            }

            // Write metrics if they are enabled:
            if (isCacheMetricRecordingEnabled)
            {
                cacheEventMetricRecorder.RecordMetrics(cacheMetrics);
            }

            // Allow the evictor to perform any cleanup:
            Teardown(cacheEvent);
        }

        /// <summary>
        /// Evicts an item from the dictionary cache by exact key. Collected metrics if they are being recorded.
        /// </summary>
        private void EvictDictionaryItem(T cacheEvent, string key)
        {
            if (isCacheMetricRecordingEnabled)
            {
                var value = HttpRuntime.Cache[key];
                var metric = new CacheEventMetric(cacheEvent.CacheEventId)
                {
                    CachedItemData = TrySerializeToJson(value),
                    CachedItemDataIsString = value is string,
                    CachedItemKey = key,
                    CacheStorageType = GetCacheStorageType(),
                    MetricType = "EVICTING"
                };
                cacheMetrics.Add(metric);
            }
            HttpRuntime.Cache.Remove(key);
        }
        
        /// <summary>
        /// Evicts data that is stored somewhere other than the dictionary cache and has
        /// become stale based on the provided cache event.
        /// 
        /// Override in derived class to perform necessary evictions, for example,
        /// removing cached HTML views from memory, or removing cached content in
        /// an edge service such as Cloudflare. 
        /// </summary>
        protected abstract void EvictNonDictionaryCacheData(T cacheEvent);

        /// <summary>
        /// Provides a list of keys of items that should be evicted from the dictionary cache
        /// based on the provided cache event.
        /// 
        /// Is higher performance than using 'IsDictionaryItemStale' method because no iteration
        /// of the dictionary cache is needed.
        /// 
        /// Override in derived class so that necessary evictions can be performed.
        /// </summary>
        protected abstract List<string> EvictSpecificDictionaryCacheKeys(T cacheEvent);

        /// <summary>
        /// Returns true if the provided cache event causes the item stored under the provided
        /// key to become stale.
        /// 
        /// Override in derived class and return true when necessary to ensure eviction of stale dictionary items.
        /// </summary>
        protected abstract bool IsDictionaryItemStale(T cacheEvent, string key);

        public Type GetCacheEventType()
        {
            return typeof(T);
        }

        protected abstract void Teardown(T cacheEvent);

        /// <summary>
        /// Only used for metrics. Has no functional effect.
        /// </summary>
        protected abstract string GetCacheStorageType();

        /// <summary>
        /// Accepts a C# object and serializes it to a JSON string if possible.
        /// </summary>
        private static string TrySerializeToJson(object value)
        {
            var valueAsString = value as string;

            if (valueAsString == null)
            {
                try
                {
                    //return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value));
                    return JsonConvert.SerializeObject(value, Formatting.Indented);
                }
                catch (Exception e)
                {
                    return "UNABLE_TO_SERIALIZE_FOR_INDEXING";
                }
            }

            try
            {
                return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(valueAsString), Formatting.Indented);
            }
            catch (Exception e)
            {
                return valueAsString;
            }
        }

        /// <summary>
        /// Keys to never remove from the dictionary cache.
        /// </summary>
        private static List<string> CacheItemNotToRemove()
        {
            var cacheItemsNotToRemove = new List<string>
            {
                "CartPromotionCache",
                "PricePromotionCache",
                "ProductPromotionCache",
                "DefaultGlobalConfigCache",
                "AllPromotionCache"
            };

            return cacheItemsNotToRemove;
        }
    }
}
