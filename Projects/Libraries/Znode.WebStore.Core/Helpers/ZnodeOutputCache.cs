using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.WebStore
{
    public class ZnodeOutputCache : OutputCacheAttribute
    {
        private const string logComponent = "ZnodeOutputCache";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (IsCacheAllowed(filterContext.HttpContext))
                {
                    base.OnActionExecuting(filterContext);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, logComponent, TraceLevel.Error);
            }
        }
        
        // To check is cache allowed or not.
        protected bool IsCacheAllowed(HttpContextBase httpContext)
        {
            try
            {
                bool SkipCacheQueryString = IsSkipCacheQueryStringExists(httpContext);
                if (PortalAgent.CurrentPortal.IsFullPageCacheActive && !SkipCacheQueryString)
                {
                    UseCache();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, logComponent, TraceLevel.Error);
                return false;
            }
        }

        // To determine cache should be skipped or not according to request query string.
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
                {
                    isSkipCache = false;
                }
            }

            //Parse for from search value
            bool.TryParse(_queryString["fromSearch"], out isFromSearch);

            return isSkipCache || isFromSearch;
        }

        /// <summary>
        ///  This is method is used to set default values for Duration and VaryByParam properties of OutputCacheAttribute.
        /// </summary>
        protected void UseCache()
        {
            if (Duration == 0)
                Duration = PortalAgent.CurrentPortal.Duration;

            if (VaryByParam == "None")
                VaryByParam = "facetGroup;pageNumber;pageId;portalId;publishState;categoryId;id;sort;pageSize;pageNumber;profileId;accountId;localeId;catalogId;expands";            
        }
    }
}