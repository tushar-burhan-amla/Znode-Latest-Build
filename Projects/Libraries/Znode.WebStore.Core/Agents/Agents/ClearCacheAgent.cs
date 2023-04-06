using DevTrends.MvcDonutCaching;
using System;
using System.Collections;
using System.Diagnostics;
using System.Web;
using Znode.Engine.Api.Client;
using Znode.Libraries.Framework.Business;
using Znode.WebStore.Caching.Helpers;

namespace Znode.Engine.WebStore.Agents
{
    public class ClearCacheAgent : BaseAgent, IClearCacheAgent
    {
        #region Private Variables
        private readonly IPortalClient _portalClient;
        private const string CacheClearedMessage = "Cache Cleared.";
        private const string CacheNotClearedMessage = "Error occurred during clear cache: ";
        #endregion

        #region Constructor
        public ClearCacheAgent(IPortalClient portalClient)
        {
            _portalClient = GetClient<IPortalClient>(portalClient);
        }
        #endregion

        #region Public Method

        //Clear Cache method.
        public virtual string ClearCache()
        {
            try
            {
                IDictionaryEnumerator cacheEnumerator = HttpContext.Current.Cache.GetEnumerator();

                //Clear all cached items.
                while (cacheEnumerator.MoveNext())
                    HttpContext.Current.Cache.Remove(cacheEnumerator.Key.ToString());

                //Clear all donut caching.
                OutputCacheManager cacheManager = new OutputCacheManager();
                cacheManager.RemoveItems();

                OutputCacheHelper.ClearOutputCache();

                return CacheClearedMessage;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return $"{CacheNotClearedMessage} {ex.Message.ToString()}";
            }
        }
        #endregion
    }
}