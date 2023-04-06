using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Parser;
using Znode.Engine.Services;

using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Cache
{
    public abstract class BaseCache
	{
		protected CacheConfiguration CacheConfig;

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
			CacheConfig = (CacheConfiguration)ConfigurationManager.GetSection("ZnodeApiCache");
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
			var json = ApiHelper.ToJson(data);

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

		protected CacheRoute GetCacheRoute(string routeTemplate)
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

            // If we have user ID then get customer profile
            if (userId > 0)
            {
                //Get the Customer Profile Details, to Set the Cache for the customer Profile.
                GetCustomerProfile(portalId, userId);
                return userId.ToString(CultureInfo.InvariantCulture);
            }

            // Otherwise use default profile
            GetDefaultProfile(portalId);
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

        private string DecodeBase64(string encodedValue) => Encoding.UTF8.GetString(Convert.FromBase64String(encodedValue));

        //Get Customer Profile based on the Portal & User Id.
        private void GetCustomerProfile(int portalId, int userId)
        {
            if (HelperUtility.IsNull(HttpContext.Current.Cache[$"ProfileCache_{userId}_{portalId}"]))
            {
                IUserService userService = ZnodeDependencyResolver.GetService<IUserService>();
                //Get the Customer Profiles based on portal & user Id. 
                List<ProfileModel> profiles = userService.GetCustomerProfile(userId, portalId);
                if (profiles?.Count > 0)
                    HttpContext.Current.Cache[$"ProfileCache_{userId}_{portalId}"] = profiles;
            }
        }

        //Get Default portal Profile Based on the Portal Id.
        private void GetDefaultProfile(int portalId)
        {
            if (HelperUtility.IsNull(HttpContext.Current.Cache[$"ProfileCache_{portalId}"]))
            {
                IUserService userService = ZnodeDependencyResolver.GetService<IUserService>();

                //Get the Customer Profile based on portal & user Id. 
                var profile = userService.GetCustomerProfile(0, portalId);
                if (HelperUtility.IsNotNull(profile))
                    HttpContext.Current.Cache[$"ProfileCache_{portalId}"] = profile;
            }
        }

        private string RemoveCacheParameterFromRouteUri(string routeUri)=> routeUri.Replace("?cache=refresh", "").Replace("&cache=refresh", "");
		
	}
}