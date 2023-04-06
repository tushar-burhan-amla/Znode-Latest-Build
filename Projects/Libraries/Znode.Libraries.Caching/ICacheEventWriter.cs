namespace Znode.Libraries.Caching
{
    public interface ICacheEventWriter
    {

        /// <summary>
        /// Write cache event to the persistent cache stream that the cache readers will read from.
        /// </summary>
        void WriteEvent(BaseCacheEvent cacheEvent);
    }
}
