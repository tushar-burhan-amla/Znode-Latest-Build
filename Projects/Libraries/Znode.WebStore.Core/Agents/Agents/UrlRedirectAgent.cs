using System;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.WebStore.Agents
{
    public class UrlRedirectAgent : BaseAgent,IUrlRedirectAgent
    {
        #region Private Variables
        private readonly IUrlRedirectClient _client;
        #endregion

        #region Constructor
        public UrlRedirectAgent(IUrlRedirectClient client)
        {
            _client = GetClient<IUrlRedirectClient>(client);
        }
        #endregion

        #region Public Methods

        //Get active 301 Url Redirects for current portal.
        public virtual UrlRedirectListModel GetActive301Redirects()
        {
            _client.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _client.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _client.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            return _client.GetUrlRedirectList(new FilterCollection { { FilterKeys.PortalId, FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString() } }, new SortCollection(), null, null);
        }

        //Checks if the current url matches the UrlRedirectFrom url.
        public static UrlRedirectViewModel Has301Redirect(string url)
        {
            //Get active 301 Url Redirects from cache.
            UrlRedirectListModel redirects = GetActive301RedirectsFromCache();

            if (HelperUtility.IsNull(redirects?.UrlRedirects))
                return null;

            var model =  redirects.UrlRedirects.FirstOrDefault(x => x.RedirectFrom.ToLower().Replace($"http://{HttpContext.Current.Request.Url.Authority}/", string.Empty) == (url.ToLower().Replace("~/", string.Empty)));

            return model.ToViewModel<UrlRedirectViewModel>();
        }
        #endregion

        #region Private Methods
        //Get active 301 Url Redirects from cache.
        private static UrlRedirectListModel GetActive301RedirectsFromCache()
        {
            string cacheKey = GenerateCacheKeyForURLRedirect();
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                IWebstoreHelper helper = GetService<IWebstoreHelper>();
                UrlRedirectListModel model = helper.GetActive301Redirects();

                if (HelperUtility.IsNotNull(model))
                    Helper.AddIntoCache(model, cacheKey, "301URLRedirectCacheDuration");
            }
            return Helper.GetFromCache<UrlRedirectListModel>(cacheKey);
        }

        //Generate the cache key to get the URLRedirect
        private static string GenerateCacheKeyForURLRedirect()
        {
            return string.Concat(WebStoreConstants.UrlRedirects + "_", Convert.ToString(PortalAgent.CurrentPortal?.PortalId));
        }
        #endregion
    }
}