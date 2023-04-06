namespace Znode.Engine.WebStore.Agents
{
    public interface IClearCacheAgent
    {
        /// <summary>
        /// This method will clear the cache.
        /// </summary>
        /// <returns>True if cache cleared.</returns>
        string ClearCache();
    }
}
