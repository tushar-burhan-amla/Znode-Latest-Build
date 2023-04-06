using System.Web;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class CacheHelper
    {
        //Add cache by cache key.
        public static void AddIntoCache<T>(T model, string cacheKey)
        {
            HttpRuntime.Cache.Insert(cacheKey, model);
        }

        //Get cache by cache key.
        public static T GetFromCache<T>(string cacheKey)
        {
            var model = HttpRuntime.Cache[cacheKey];
            if (model is T)
            {
                try
                {
                    return (T)model;
                }
                catch
                {
                    return default(T);
                }

            }
            return default(T);
        }

        //Clear cache by cache key.
        public static void ClearCache(string cacheKey)
        {
            var obj = GetFromCache<object>(cacheKey);
            if (obj == null) return;
            HttpContext.Current.Cache.Remove(cacheKey);
        }
    }
}
