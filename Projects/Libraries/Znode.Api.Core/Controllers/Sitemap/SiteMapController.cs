using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Api.Core.Controllers
{
    public class SiteMapController : BaseController
    {

        #region Private Variables   
        private readonly ISiteMapCache _siteMapCache;
        #endregion

        #region Default Constructor
        public SiteMapController()
        {
            
        }
        public SiteMapController(ISiteMapService siteMapService)
        {          
            _siteMapCache = new SiteMapCache(siteMapService);
        }
        #endregion

        /// <summary>
        /// Gets list of published categories.
        /// </summary>
        /// <param name="includeAssociatedCategories">
        /// This parameter is used to include the child categories.
        /// if includeAssociatedCategories  is true then all the categories of parent product
        /// is included otherwise only parent categories will display.
        /// Default value for includeAssociatedCategories is true.
        /// </param>
        /// <returns>Returns list of categories.</returns>   
        [ResponseType(typeof(WebStoreCategoryListResponse))]
        [HttpGet]
        public HttpResponseMessage GetSiteMapCategoryList(bool includeAssociatedCategories = true)
        {
            HttpResponseMessage response;

            try
            {
                //Get data from cache
                string data = _siteMapCache.GetSiteMapCategoryList(includeAssociatedCategories, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreCategoryListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreCategoryListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of brands.
        /// </summary>
        /// <returns>It will return the list of brands.</returns>
        [ResponseType(typeof(BrandListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSiteMapBrandList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _siteMapCache.GetSiteMapBrandList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BrandListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new BrandListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of Products.
        /// </summary>
        /// <returns>Return product list.</returns>
        [ResponseType(typeof(PublishProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSiteMapProductList()
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute by attribute id.
                string data = _siteMapCache.GetSiteMapProductList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }
    }
}
