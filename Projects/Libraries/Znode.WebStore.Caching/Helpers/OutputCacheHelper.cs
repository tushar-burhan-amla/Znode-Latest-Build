using System.Runtime.Caching;
using System.Web.Mvc;

namespace Znode.WebStore.Caching.Helpers
{
    public class OutputCacheHelper
    {
        /// <summary>
        /// To register new memory cache instace for output cache to avoid use of instance provided by MemoryCache.Default property.
        /// </summary>
        public static void RegisterMemoryCache()
        {
            OutputCacheAttribute.ChildActionCache = new MemoryCache("ZnodeOutputCache");
        }

        /// <summary>
        /// To clear entire output cache for child actions.
        /// </summary>
        public static void ClearOutputCache()
        {
            ((MemoryCache)OutputCacheAttribute.ChildActionCache).Dispose();
            OutputCacheAttribute.ChildActionCache = new MemoryCache("ZnodeOutputCache");
        }
    }
}
