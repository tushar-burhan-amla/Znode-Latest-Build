using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class ContentContainerCache : BaseCache, IContentContainerCache
    {

        #region Private Variables
        private readonly IContentContainerService _service;
        #endregion

        #region Constructor
        public ContentContainerCache(IContentContainerService contentContainerService)
        {
            _service = contentContainerService;
        }
        #endregion

        //Get List
        public virtual string List(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ContentContainerListModel list = _service.List(Expands, Filters, Sorts, Page);
                if (list?.ContainerList?.Count > 0)
                {
                    //Create response.
                    ContentContainerListResponseModel response = new ContentContainerListResponseModel { ContainerList = list.ContainerList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Content Container
        public virtual string GetContentContainer(string containerKey, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                ContentContainerResponseModel model = _service.GetContentContainer(containerKey);
                if (HelperUtility.IsNotNull(model))
                {
                    ContentContainerResponse response = new ContentContainerResponse { ContentContainerModel = model };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Associated Variant
        public virtual string GetAssociatedVariants(string containerKey, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {

                List<AssociatedVariantModel> model = _service.GetAssociatedVariants(containerKey);
                if (HelperUtility.IsNotNull(model))
                {
                    ContentContainerListResponseModel response = new ContentContainerListResponseModel { AssociatedVariants = model };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }

            }
            return data;
        }

        //Get Content Container Data
        public virtual string GetContentContainerData(string routeUri, string routeTemplate, string containerKey, int localeId, int portalId = 0, int profileId = 0)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Content container Attribute Values.
                ContentContainerDataModel attributeDetails = _service.GetContentContainerData(containerKey, localeId, portalId, profileId);
                //Get response and insert it into cache.
                ContentContainerDataResponse response = new ContentContainerDataResponse { ContentContainerData = attributeDetails };
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get List
        public virtual string GetAssociatedVariantList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AssociatedVariantListModel list = _service.GetAssociatedVariantList(Expands, Filters, Sorts, Page);
                if (list?.AssociatedVariants?.Count > 0)
                {
                    //Create response.
                    ContentContainerListResponseModel response = new ContentContainerListResponseModel { AssociatedVariants = list.AssociatedVariants };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Content Container
        public virtual string GetVariantLocaleData(int variantId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                ContentContainerResponseModel model = _service.GetVariantLocaleData(variantId);
                if (HelperUtility.IsNotNull(model))
                {
                    ContentContainerResponse response = new ContentContainerResponse { ContentContainerModel = model };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}
