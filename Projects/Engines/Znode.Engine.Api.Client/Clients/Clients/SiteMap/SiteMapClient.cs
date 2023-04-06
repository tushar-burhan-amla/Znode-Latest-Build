using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class SiteMapClient : BaseClient, ISiteMapClient
    {
        #region Public Method.

        // Get sitemap category list.
        public virtual SiteMapCategoryListModel GetSitemapCategoryList(bool includeAssociatedCategories, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = SiteMapEndpoint.GetSitemapCategoryList(includeAssociatedCategories);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            SiteMapCategoryListResponse response = GetResourceFromEndpoint<SiteMapCategoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SiteMapCategoryListModel list = new SiteMapCategoryListModel();
            if (HelperUtility.IsNotNull(response))
            {
                list.CategoryList = response.CategoryList;
                list.TotalResults = response.TotalResults;
            }
            list.MapPagingDataFromResponse(response);

            return list;
        }


        // Get sitemap brand list.
        public virtual SiteMapBrandListModel GetSitemapBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = SiteMapEndpoint.GetSitemapBrandList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            SiteMapBrandListResponce response = GetResourceFromEndpoint<SiteMapBrandListResponce>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SiteMapBrandListModel list = new SiteMapBrandListModel { BrandList = response?.BrandList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Get sitemap product list.
        public virtual SiteMapProductListModel GetSitemapProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = SiteMapEndpoint.GetSitemapProductList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            SiteMapProductListResponse response = GetResourceFromEndpoint<SiteMapProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SiteMapProductListModel list = new SiteMapProductListModel { ProductList = response?.ProductList.Select(x => new SiteMapProductModel { Name = x.Name, ZnodeProductId = x.ZnodeProductId, CategoryName = x.CategoryName, SEOUrl = x.SEOUrl }).ToList() };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get product feed by portal Id.
        public virtual List<ProductFeedModel> GetProductFeedByPortalId(int portalId)
        {
            //Create Endpoint to get the list of all product feed by portalId.
            string endpoint = ProductFeedEndpoint.GetProductFeedByPortalId(portalId);
            ApiStatus status = new ApiStatus();

            ProductFeedListResponse response = GetResourceFromEndpoint<ProductFeedListResponse>(endpoint, status);
            //Check the status of response of product feed list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            
            return response?.ProductFeeds;
        }

        #endregion
    }
}
