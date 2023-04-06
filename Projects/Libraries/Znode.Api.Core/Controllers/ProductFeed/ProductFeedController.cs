using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Controllers
{
    public class ProductFeedController : BaseController
    {
        #region Private Variables

        private readonly IProductFeedService _service;
        private readonly IProductFeedCache _cache;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for Google Site Map
        /// </summary>
        public ProductFeedController(IProductFeedService service)
        {
            _service = service;
            _cache = new ProductFeedCache(_service);
        }

        #endregion

        #region Public Method
        /// <summary>
        /// Generates an XML file 
        /// </summary>
        /// <param name="model">Product Feed Model</param>
        /// <returns> It will return an Xml file which content all the detail about the product,category and content</returns>
        [ResponseType(typeof(ProductFeedResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage Create([FromBody] ProductFeedModel model)
        {
            HttpResponseMessage response;
            try
            {
                ProductFeedModel productFeed = _service.CreateGoogleSiteMap(model);
                if (HelperUtility.IsNotNull(productFeed))
                {
                    Uri uri = Request.RequestUri;
                    string location = uri.Scheme + "://" + uri.Host + uri.AbsolutePath + "/" + productFeed.ProductFeedId;

                    response = CreateCreatedResponse(new ProductFeedResponse { ProductFeed = productFeed });
                    response.Headers.Add("Location", location);
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (Exception ex )
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ProductFeedResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }



        /// <summary>
        ///  Get Product Feed MasterDetails
        /// </summary>
        /// <returns>Returns the product feed master details.</returns>
        [ResponseType(typeof(OrderStateResponses))]
        [HttpGet]
        public HttpResponseMessage GetProductFeedMasterDetails()
        {
            HttpResponseMessage response;
            try
            {
                ProductFeedModel model = _service.GetProductFeedMasterDetails();
                response = IsNotNull(model) ? CreateCreatedResponse(new ProductFeedResponse { ProductFeed = model }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderStateResponses { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of product feed.
        /// </summary>
        /// <returns>Return List of product Feed</returns>
        [ResponseType(typeof(ProductFeedListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetProductFeedList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductFeedListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new ProductFeedListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Method to update product feed.
        /// </summary>
        /// <param name="model">ProductFeed model.</param>
        /// <returns>Returns updated ProductFeed.</returns>
        [ResponseType(typeof(ProductFeedResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Update([FromBody] ProductFeedModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update ProductFeed.
                bool IsUpdated = _service.UpdateProductFeed(model);
                if (IsUpdated)
                {
                    response = CreateCreatedResponse(new ProductFeedResponse { ProductFeed = model });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.ProductFeedId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ProductFeedResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ProductFeedResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;

        }

        /// <summary>
        /// Gets a product feed by id.
        /// </summary>
        /// <param name="productFeedId">The id of the productfeed.</param>
        /// <returns>return ProductFeed.</returns>
        [ResponseType(typeof(ProductFeedResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int productFeedId)
        {
            HttpResponseMessage response;
            try
            {
                //Get ProductFeed by ProductFeed id.
                string data = _cache.GetProductFeed(productFeedId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductFeedResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ProductFeedResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Delete a Product feed by Id.
        /// </summary>
        /// <param name="productFeedIds">Id of Product feed to delete Product feed.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteProductFeed([FromBody] ParameterModel productFeedIds)
        {
            HttpResponseMessage response;
            try
            {
                //Delete product feed.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteProductFeed(productFeedIds) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get product feed by portal Id.
        /// </summary>
        /// <param name="portalId">portalId</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(ProductFeedListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetProductFeedByPortalId(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetProductFeedByPortalId(RouteUri, RouteTemplate, portalId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductFeedListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ProductFeedListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ProductFeedListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Check if the file name combination already exists.
        /// </summary>
        /// <param name="localeId">localeId</param>
        /// <param name="fileName">fileName</param>
        /// <returns>returns true if combination exist</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage FileNameCombinationAlreadyExist(int localeId, string fileName = "")
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.FileNameCombinationAlreadyExist(localeId, fileName) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            return response;
        }

        #endregion

    }
}