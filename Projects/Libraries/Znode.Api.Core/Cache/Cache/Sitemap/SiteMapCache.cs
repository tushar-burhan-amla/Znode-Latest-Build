using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class SiteMapCache : BaseCache, ISiteMapCache
    {
        #region Global Variable
        private readonly ISiteMapService _siteMapService;
        #endregion

        #region Default Constructor
        public SiteMapCache(ISiteMapService siteMapService)
        {
            _siteMapService = siteMapService;
        }
        #endregion

        #region Public Methods

        //Get a list of Categories,SubCategories and Product list.
        public virtual string GetSiteMapCategoryList(bool includeAssociatedCategories, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache($"{routeUri}{includeAssociatedCategories}");
            if (HelperUtility.IsNull(data))
            {
                //Get Categories,SubCategories and associated Product list
                SiteMapCategoryListModel list = _siteMapService.GetSiteMapCategoryList(includeAssociatedCategories, Expands, Filters, Sorts, Page);
                if (list?.CategoryList?.Count > 0)
                {
                    //Create response.
                    SiteMapCategoryListResponse response = new SiteMapCategoryListResponse { CategoryList = list.CategoryList, TotalResults= list.TotalResults};
                   
                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache($"{routeUri}{includeAssociatedCategories}", routeTemplate, response);
                }
            }
            return data;
        }


        //Get a list of brands.
        public virtual string GetSiteMapBrandList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get brand list
                SiteMapBrandListModel list = _siteMapService.GetSiteMapBrandList(Expands, Filters, Sorts, Page);
                if (list?.BrandList?.Count > 0)
                {
                    //Create response.
                    SiteMapBrandListResponce response = new SiteMapBrandListResponce { BrandList = list.BrandList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }


        //Get list of Publish Product.
        public virtual string GetSiteMapProductList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Publish product list.                
                SiteMapProductListModel list = _siteMapService.GetSiteMapProductList(Expands, Filters, Sorts, Page);

                if (list?.ProductList?.Count > 0)
                {
                    //Create response.
                    SiteMapProductListResponse response = new SiteMapProductListResponse { ProductList = list.ProductList, TotalResults = list.TotalResults };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
