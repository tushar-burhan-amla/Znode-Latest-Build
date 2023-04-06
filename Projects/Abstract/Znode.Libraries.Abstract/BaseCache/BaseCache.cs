using Newtonsoft.Json;

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using Znode.Libraries.Abstract.Parser;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.Abstract.Cache
{
    public abstract class BaseCache
	{
		protected Znode.Libraries.ECommerce.Utilities.CacheConfiguration CacheConfig;

		protected QueryStringParser QueryStringParser;

		protected NameValueCollection Expands => QueryStringParser.Expands; 	

		protected FilterCollection Filters => QueryStringParser.Filters;

        protected FilterCollection MongoFriendlyFilters => QueryStringParser.MongoFriendlyFilters;
		
		protected NameValueCollection Sorts=> QueryStringParser.Sorts;

        protected NameValueCollection MongoFriendlySorts => QueryStringParser.MongoFriendlySorts;

        protected NameValueCollection Page => QueryStringParser.Page;		

		protected NameValueCollection Cache => QueryStringParser.Cache;		

		protected bool RefreshCache
		{
			get
			{
				var refreshCache = false;

				if (Cache.HasKeys() && !String.IsNullOrEmpty(Cache.Get("refresh")))
                    refreshCache = true;

                return refreshCache;
			}
		}

		protected BaseCache()
		{
			CacheConfig = (Znode.Libraries.ECommerce.Utilities.CacheConfiguration)ConfigurationManager.GetSection("ZnodeApiCache");
			if (CacheConfig == null)
			{
				throw new ConfigurationErrorsException("Configuration section for ZnodeApiCache does not exist.");
			}

			QueryStringParser = new QueryStringParser(HttpContext.Current.Request.Url.Query);
		}

		protected string GetFromCache(string routeUri)
		{
			// IMPORTANT: Must remove cache=refresh from the route
			routeUri = RemoveCacheParameterFromRouteUri(routeUri);

			// If cache=refresh for the route then remove the value from cache so that the
			// call is forced to retrieve data from the service and re-insert into cache.
			if (RefreshCache) HttpRuntime.Cache.Remove(routeUri);

			// If cache=refresh was used then this will always return null, which is the
			// expected result.
			return HttpRuntime.Cache.Get(routeUri) as string;
		}

		protected string InsertIntoCache(string routeUri, string routeTemplate, object data)
		{
			// IMPORTANT: Must remove cache=refresh from the route
			routeUri = RemoveCacheParameterFromRouteUri(routeUri);

			// We serialize the data as a string because we want to store just the JSON string
			// itself in the cache, not its response object. This is a big performance gain.
			var json = ToJson(data);

			// Otherwise check if caching is enabled overall and for the route itself
			if (CacheConfig.Enabled)
			{
				var cacheRoute = GetCacheRoute(routeTemplate);
				if (cacheRoute != null && cacheRoute.Enabled)
				{
                    if (cacheRoute.Sliding)
                    {
                        HttpRuntime.Cache.Insert(routeUri, json, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(cacheRoute.Duration));
                    }
                    else
                    {
                        // IMPORTANT: Use UtcNow to avoid issues with local time, such as changes for daylight savings
                        HttpRuntime.Cache.Insert(routeUri, json, null, DateTime.Now.AddSeconds(cacheRoute.Duration), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                }
			}
			return json;
		}
		//This method respects the http header value to optimize JSON response from APIs
		//Sets the Newtonsoft JSON conversion setting, to minify the serialized JSON to ignore null and default value properties.
		//So each API response which we are serializing using this method will use these settings.
		protected string ToJson(object data)
		{
			bool minifiedJsonResponse = ZnodeApiSettings.MinifiedJsonResponse;
			return JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
			{
				DefaultValueHandling = minifiedJsonResponse ? DefaultValueHandling.Ignore : DefaultValueHandling.Include
			});
		}
		protected Znode.Libraries.ECommerce.Utilities.CacheRoute GetCacheRoute(string routeTemplate)
		{
			if (!String.IsNullOrEmpty(routeTemplate))
			{
				foreach (var item in CacheConfig.CacheRoutes)
				{
					if (item.Template == routeTemplate)
					{
						return item;
					}
				}
			}

			return null;
		}

        //Gets the user ID and portal ID from the request headers and puts the profile into cache.
        protected string UpdateCacheForProfile()
        {
            const string headerUserId = "Znode-UserId";
            int userId = 0;
            int portalId = 1;

            // Get user ID and domain name from request header
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerUserId], out userId);
            string domainName = GetDomainNameFromAuthHeader();

            if (!String.IsNullOrEmpty(domainName))
            {
                var domain = ZnodeConfigManager.GetSiteConfigFromCache(domainName);
                if (HelperUtility.IsNotNull(domain))                
                    portalId = domain.PortalId;                
            }

           
            return String.Empty;
        }

        //Get the Domain Name from the Request.
        private string GetDomainNameFromAuthHeader()
		{
			var headers = HttpContext.Current.Request.Headers;

		    const string domainHeader = "Znode-DomainName";

		    string domain = string.IsNullOrEmpty(headers[domainHeader]) ? HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").Trim() : headers[domainHeader].ToString();

		    return domain;
        }

       
        private string RemoveCacheParameterFromRouteUri(string routeUri)=> routeUri.Replace("?cache=refresh", "").Replace("&cache=refresh", "");
		
	}
}