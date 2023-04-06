using System.Collections.Generic;

namespace Znode.Libraries.Caching
{
    public interface ICacheEventMetricRecorder
    {

        /// <summary>
        /// Write cache event metric.
        /// </summary>
        void RecordMetric(ICacheEventMetric cacheEventMetric);

        /// <summary>
        /// Write cache event metric.
        /// </summary>
        void RecordMetrics(List<ICacheEventMetric> cacheEventMetrics);
    }
}
