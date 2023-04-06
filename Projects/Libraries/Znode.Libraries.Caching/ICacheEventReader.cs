using System;
using System.Collections.Generic;

namespace Znode.Libraries.Caching
{
    public interface ICacheEventReader
    {

        /// <summary>
        /// Get all cache events that were created in the provided time range.
        /// </summary>
        List<BaseCacheEvent> ReadEvents(DateTime minDateTime, DateTime maxDateTime);
    }
}
