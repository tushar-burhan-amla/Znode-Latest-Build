using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class PublishCategoryCache : BaseCache, IPublishCategoryCache
    {
        #region Private Variable
        private readonly IPublishCategoryService _service;
        #endregion

        #region Constructor
        public PublishCategoryCache(IPublishCategoryService publishCategoryService)
        {
            _service = publishCategoryService;
        }
        #endregion

        #region Public Methods

        //Get list of Publish Category.
        public virtual string GetPublishCategoryList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Publish Category list.
                PublishCategoryListModel list = _service.GetPublishCategoryList(Expands, Filters, Sorts, Page);

                if (IsNotNull(list?.PublishCategories))
                {
                    //Create response.
                    PublishCategoryListResponse response = new PublishCategoryListResponse { PublishCategories = list.PublishCategories };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of publish category excluding assigned ids.
        public virtual string GetUnAssignedPublishCategoryList(string assignedIds, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);

            if (IsNull(data))
            {
                //Get Publish Category list.
                PublishCategoryListModel list = _service.GetUnAssignedPublishCategoryList(assignedIds, Expands, Filters, Sorts, Page);

                if (list?.PublishCategories?.Count > 0)
                {
                    //Create response.
                    PublishCategoryListResponse response = new PublishCategoryListResponse { PublishCategories = list.PublishCategories };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Publish Category by Publish Category id.
        public virtual string GetPublishCategory(int publishCategoryId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                PublishCategoryModel publishCategory = _service.GetPublishCategory(publishCategoryId, Filters, Expands);
                if (IsNotNull(publishCategory))
                {
                    PublishCategoryResponse response = new PublishCategoryResponse { PublishCategory = publishCategory };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}