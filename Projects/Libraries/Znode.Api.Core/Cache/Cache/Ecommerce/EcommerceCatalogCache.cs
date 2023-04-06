using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
namespace Znode.Engine.Api.Cache
{
    public class EcommerceCatalogCache : BaseCache, IEcommerceCatalogCache
    {
        #region Private Variable
        private readonly IEcommerceCatalogService _service;
        #endregion

        #region Constructor
        public EcommerceCatalogCache(IEcommerceCatalogService ecommerceCatalogService)
        {
            _service = ecommerceCatalogService;
        }
        #endregion

        #region Public Methods
        public virtual string GetPublishCatalogList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                PublishCatalogListModel publishCatalogList = _service.GetPublishCatalogList(Expands, Filters, Sorts, Page);
                if (publishCatalogList?.PublishCatalogs?.Count > 0)
                {
                    //Create Response and insert in to cache
                    PublishCatalogListResponse response = new PublishCatalogListResponse { PublishCatalogs = publishCatalogList.PublishCatalogs };

                    response.MapPagingDataFromModel(publishCatalogList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //method to get catalogs associated with portal as per portalId.
        public virtual string GetAssociatedPortalCatalogByPortalId(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PortalCatalogListModel list = _service.GetAssociatedPortalCatalogByPortalId(portalId, Expands, Filters, Sorts, Page);
                if (list?.PortalCatalogs?.Count > 0)
                {
                    PortalCatalogListResponse response = new PortalCatalogListResponse { PortalCatalogs = list?.PortalCatalogs };

                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //method Gets Portal catalog. 
        public virtual string GetPortalCatalog(int portalCatalogId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PortalCatalogModel portalCatalogModel = _service.GetPortalCatalog(portalCatalogId);
                if (portalCatalogModel?.PortalCatalogId > 0)
                {
                    PortalCatalogResponse response = new PortalCatalogResponse { PortalCatalog = portalCatalogModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //method Gets Publish Catalog Details
        public virtual string GetPublishCatalogDetails(int publishCatalogId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PublishCatalogModel publishCatalogModel = _service.GetPublishCatalogDetails(publishCatalogId);
                if (publishCatalogModel?.PublishCatalogId > 0)
                {
                    EcommerceResponse response = new EcommerceResponse { PublishCatalog = publishCatalogModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //method Gets Publish Category Details
        public virtual string GetPublishCategoryDetails(int publishCategoryId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PublishCategoryModel publishCategoryModel = _service.GetPublishCategoryDetails(publishCategoryId);
                if (publishCategoryModel?.PublishCategoryId > 0)
                {
                    EcommerceResponse response = new EcommerceResponse { PublishCategory = publishCategoryModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //method Gets Publish Product Details
        public virtual string GetPublishProductDetails(int publishProductId, int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PublishProductModel publishProductModel = _service.GetPublishProductDetails(publishProductId, portalId);
                if (publishProductModel?.PublishProductId > 0)
                {
                    EcommerceResponse response = new EcommerceResponse { PublishProduct = publishProductModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}