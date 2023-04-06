using System.Collections.Generic;
using Znode.Libraries.Caching.ElasticSearch;

namespace Znode.Libraries.Caching
{
    public class ElasticsearchCacheEventMetricRecorder : ICacheEventMetricRecorder
    {
        public void RecordMetric(ICacheEventMetric cacheEventMetric)
        {
            // TODO - add buffer support ?
            var cacheEventMetrics = new List<ICacheEventMetric>();
            cacheEventMetrics.Add(cacheEventMetric);
            RecordMetrics(cacheEventMetrics);
        }

        public void RecordMetrics(List<ICacheEventMetric> cacheEventMetrics)
        {
            ElasticSearchHelper.IndexDocuments(CacheFrameworkSettings.GetCacheMetricsIndexName(), cacheEventMetrics, typeof(CacheEventMetric).Name);
        }
    }
}
