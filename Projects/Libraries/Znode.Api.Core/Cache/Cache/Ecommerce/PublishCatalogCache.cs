using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class PublishCatalogCache : BaseCache, IPublishCatalogCache
    {
        #region Private Variable
        private readonly IPublishCatalogService _service;
        #endregion

        #region Constructor
        public PublishCatalogCache(IPublishCatalogService publishCatalogService)
        {
            _service = publishCatalogService;
        }
        #endregion

        #region Public Methods

        //Get list of Publish Catalog.
        public virtual string GetPublishCatalogList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get published catelog list .
                PublishCatalogListModel list = _service.GetPublisCatalogList(Expands, Filters, Sorts, Page);

                if (list?.PublishCatalogs?.Count > 0)
                {
                    //Create response.
                    PublishCatalogListResponse response = new PublishCatalogListResponse { PublishCatalogs = list.PublishCatalogs };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of publish category excluding assigned ids.
        public virtual string GetUnAssignedPublishCatelogList(string assignedIds, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);

            if (IsNull(data))
            {
                //Get Publish Category list.
                PublishCatalogListModel list = _service.GetUnAssignedPublishCatelogList(assignedIds, Expands, Filters, Sorts, Page);

                if (list?.PublishCatalogs?.Count > 0)
                {
                    //Create response.
                    PublishCatalogListResponse response = new PublishCatalogListResponse { PublishCatalogs = list.PublishCatalogs };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get PublisCatalog by Publish Catalog id.
        public virtual string GetPublishCatalog(int publishCatalogId, int? localeId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                PublishCatalogModel publishCatalog = _service.GetPublisCatalog(publishCatalogId, localeId, Expands);
                if (IsNotNull(publishCatalog))
                {
                    PublishCatalogResponse response = new PublishCatalogResponse { PublishCatalog = publishCatalog };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}