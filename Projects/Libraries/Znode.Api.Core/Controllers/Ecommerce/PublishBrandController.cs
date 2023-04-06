using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers.Ecommerce
{
    public class PublishBrandController : BaseController
    {
        #region Private Variables

        private readonly IPublishBrandCache _cache;

        #endregion

        #region Constructor
        public PublishBrandController(IPublishBrandService service)
        {
            _cache = new PublishBrandCache(service);
        }
        #endregion

        #region Public Methods     

        /// <summary>
        /// Gets list of brands.
        /// </summary>
        /// <returns>It will return the list of brands</returns>
        [ResponseType(typeof(BrandListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishBrandList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishBrandList(RouteUri, RouteTemplate);
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
        /// Gets a brand by brandId.
        /// </summary>
        /// <param name="brandId">ID of brand to be retrieved.</param>
        /// <param name="localeId">ID of local </param>
        /// <param name="portalId"></param>
        /// <returns>It will return brand base on brand Id</returns>
        [ResponseType(typeof(BrandResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishBrand(int brandId, int localeId, int portalId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishBrand(brandId, localeId, portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BrandResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                BrandResponse data = new BrandResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }
        #endregion
    }
}