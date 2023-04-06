using System;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class MediaConfigurationCache : BaseCache, IMediaConfigurationCache
    {
        private readonly IMediaConfigurationService _service;

        //Constructor
        public MediaConfigurationCache(IMediaConfigurationService mediaConfigurationService)
        {
            _service = mediaConfigurationService;
        }

        //Get Media server list.
        public virtual string GetMediaServers(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                var list = _service.GetMediaServers(Expands, Filters, Sorts, Page);
                if (list?.MediaServers?.Count > 0)
                {
                    MediaConfigurationResponse response = new MediaConfigurationResponse { MediaServers = list.MediaServers };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get media configuration.
        public virtual string GetMediaConfiguration(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            bool removeSecretKey = false;
            if (string.IsNullOrEmpty(data))
            {
                var mediaConfiguration = _service.GetMediaConfiguration(Filters, Expands);
                if (HelperUtility.IsNotNull(mediaConfiguration))
                {
                    if (Filters.Exists(x => x.FilterName == FilterKeys.Caller))
                    {
                        removeSecretKey = true;
                        Filters.Remove(Filters.Find(x => x.FilterName == FilterKeys.Caller));
                    }
                    // We cannot show secret key to the user hence hiding it, however user can update it from the page.
                    if (removeSecretKey) mediaConfiguration.SecretKey = "";
                    MediaConfigurationResponse response = new MediaConfigurationResponse { MediaConfiguration = mediaConfiguration };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                  
                }
            }
            return data;
        }

        //Get default media configuration.
        public virtual string GetDefaultMediaConfiguration(string routeUri, string routeTemplate)
        {
            var data = GetFromCache(routeUri);
            if (HelperUtility.IsNull(data))
            {
                var mediaConfiguration = _service.GetDefaultMediaConfiguration();
                if (HelperUtility.IsNotNull(mediaConfiguration))
                {
                    MediaConfigurationResponse response = new MediaConfigurationResponse { MediaConfiguration = mediaConfiguration };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                
                }
            }
            return data;
        }

        //Get media count
        public virtual string GetMediaCount(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get media count
                int mediaCount = _service.GetMediaCount();

                //Create response.()
                StringResponse response = new StringResponse { Response = Convert.ToString(mediaCount) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get media list data for generate images.
        public virtual string GetMediaListData(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get media's from database by call service method
                MediaManagerListModel list = _service.GetMediaListData(Expands, Filters, Sorts, Page);
                if (HelperUtility.IsNotNull(list))
                {
                    //Get response and insert it into cache.                   
                    MediaManagerListResponses response = new MediaManagerListResponses { MediaList = list };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}