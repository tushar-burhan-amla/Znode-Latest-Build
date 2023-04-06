using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Znode.Libraries.Caching;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Api.Caching.Core
{
    internal abstract class BaseApiEvictor<T> : BaseCacheEventEvictor<T> where T : BaseCacheEvent, new()
    {
        /// <summary>
        /// Configuration specific to each API endpoint's caching behavior. This comes from the cache.config file in the API directory.
        /// </summary>
        private static CacheConfiguration CacheConfig = null;

        private static readonly string ErrorMessage = "Failed to load cache.config. Is this being used from a non-API process?";

        static BaseApiEvictor()
        {
            CacheInitializer.EnsureInitialized();
            try
            {
                CacheConfig = (CacheConfiguration)ConfigurationManager.GetSection("ZnodeApiCache");
            }
            catch (Exception e)
            {
                ZnodeLogging.LogMessage(ErrorMessage, CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error, e);
            }
        }

        /// <summary>
        /// Get the base URL of the endpoint specified by this route, from the cache.config file in the API directory.
        ///
        /// For example, if the following entry is in the cache.config:
        ///   <route template="webstoreproducts/get/{productId}" key="webstoreproduct" enabled="true" sliding="true" duration="3600" />
        ///
        /// And if the templateKey of "webstoreproduct" is provided, the return value with be "webstoreproducts/get/"
        /// </summary>
        protected static string GetRouteFromTemplateKey(string templateKey)
        {
            if (CacheConfig == null)
            {
                ZnodeLogging.LogMessage(ErrorMessage, CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error);
            }

            string template = CacheConfig?.CacheRoutes.FirstOrDefault(x => x.Key == templateKey)?.Template;
            if (template != null)
                return template.LastIndexOf('{') > 0 ? template?.Substring(0, template.LastIndexOf('{')) : template; // Should this be 'FirstIndexOf'?

            return string.Empty;
        }
        protected override string GetCacheStorageType()
        {
            return "API";
        }
    }
}
