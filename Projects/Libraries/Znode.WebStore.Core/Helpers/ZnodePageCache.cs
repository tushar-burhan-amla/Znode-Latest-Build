using DevTrends.MvcDonutCaching;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Znode.Engine.WebStore.Agents;
using System.Collections.Specialized;
using System.Collections.Generic;
namespace Znode.Engine.WebStore
{
    public class ZnodePageCache : DonutOutputCacheAttribute
    {
        private OutputCacheLocation? originalLocation;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ValidatePageCache(filterContext);
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ValidatePageCache(filterContext);
            base.OnResultExecuting(filterContext);
        }

        #region Protected Methods

        /// <summary>
        /// Returns true or false whether to Use cache or not
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected bool IsCacheAllowed(HttpContextBase httpContext)
        {
            bool SkipCacheQueryString = IsSkipCacheQueryStringExists(httpContext);
            if (PortalAgent.CurrentPortal.IsFullPageCacheActive && !SkipCacheQueryString)
                return true;
            return false;
        }

        /// <summary>
        ///  Use cache
        /// </summary>
        protected void UseCache()
        {
            Location = originalLocation ?? Location;
            Duration = Duration == 0 ? PortalAgent.CurrentPortal.Duration : Duration;
            VaryByParam = VaryByParam == "None" ? "facetGroup;pageNumber;pageId;portalId;publishState;categoryId;id;sort;pageSize;pageNumber;profileId;accountId;localeId;catalogId;expands" : VaryByParam;
        }

        /// <summary>
        /// Do not set cache
        /// </summary>
        protected virtual void DoNotUseCache()
        {
            originalLocation = originalLocation ?? Location;
            Location = OutputCacheLocation.None;
            NoStore = true;
            Duration = 0;            
            VaryByParam = "None";
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Check if Skip Cache Query String Exists
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private bool IsSkipCacheQueryStringExists(HttpContextBase httpContext)
        {
            List<string> queryStringKeys = new List<string> { "pagenumber", "sort", "viewAll", "cmsmode", "pageSize" };

            bool isSkipCache = false;
            bool isFromSearch;

            NameValueCollection _queryString = httpContext.Request.QueryString;

            foreach (string item in queryStringKeys)
            {
                if (_queryString[item] != null)
                {
                    isSkipCache = true;
                    break;
                }
                else
                    isSkipCache = false;
            }

            //Parse for from search value
            bool.TryParse(_queryString["fromSearch"], out isFromSearch);

            return isSkipCache || isFromSearch;
        }

        /// <summary>
        /// Validate Page Cache
        /// </summary>
        private void ValidatePageCache(ControllerContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            if (IsCacheAllowed(httpContext))
                UseCache();
            else
                DoNotUseCache();
        }
        #endregion
    }
}
