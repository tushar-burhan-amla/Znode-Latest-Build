using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class HighlightController : BaseController
    {
        private readonly IHighlightService _service;
        private readonly IHighlightCache _cache;

        public HighlightController(IHighlightService service)
        {
            _service = service;
            _cache = new HighlightCache(_service);
        }

        /// <summary>
        /// Gets a list of Highlight.
        /// </summary>
        /// <returns>HttpResponseMessage</returns>   
        [ResponseType(typeof(HighlightListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetHighlights(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<HighlightListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString() ,TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new HighlightListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #region Highlight Type
        /// <summary>
        /// Gets list of Highlight type.
        /// </summary>
        /// <returns>Returns highlight type list.</returns>
        [ResponseType(typeof(HighlightTypeListResponse))]
        [HttpGet]
        public HttpResponseMessage GetHighlightTypeList()
        {
            HttpResponseMessage response;
            try
            {
                //Get shipping rule types.
                string data = _cache.GetHighlightTypeList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<HighlightTypeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new HighlightTypeListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #endregion

        /// <summary>
        /// Creates a new highlight.
        /// </summary>
        /// <param name="highlightModel">The model of the Highlight.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(HighlightResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] HighlightModel highlightModel)
        {
            HttpResponseMessage response;
            try
            {
                HighlightModel Highlight = _service.CreateHighlight(highlightModel);
                if (HelperUtility.IsNotNull(Highlight))
                {
                    response = CreateCreatedResponse(new HighlightResponse { Highlight = Highlight });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(Highlight.HighlightId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new HighlightResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new HighlightResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Gets highlight.
        /// </summary>
        /// <param name="highlightId">ID of the highlight.</param>
        /// <param name="productId">ID of the Product.</param>
        /// <returns>Highlight.</returns>
        [ResponseType(typeof(HighlightResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int highlightId, int productId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetHighlight(highlightId, productId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<HighlightResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new HighlightResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
            }

            return response;
        }

        /// <summary>
        /// Gets highlight by highlight code.
        /// </summary>
        /// <param name="highLightCode">Code of the highlight.</param>
        /// <returns>Highlight.</returns>
        [ResponseType(typeof(HighlightResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetByCode(string highLightCode)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetHighlightByCode(highLightCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<HighlightResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new HighlightResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
            }

            return response;
        }

        /// <summary>
        /// Updates an existing highlight.
        /// </summary>        
        /// <param name="highlightModel">HighlightModel</param>
        /// <returns> Updates an existing highlight.</returns>
        [ResponseType(typeof(HighlightResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage Update([FromBody] HighlightModel highlightModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update highlight.
                bool isUpdated = _service.UpdateHighlight(highlightModel);
                response = isUpdated ? CreateOKResponse(new HighlightResponse { Highlight = highlightModel }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(highlightModel.HighlightId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new HighlightResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new HighlightResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete Highlight.
        /// </summary>
        /// <param name="highlightId">Highlight Id.</param>
        /// <returns>Returns true if deleted sucessfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel highlightId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteHighlight(highlightId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of highlight code.
        /// </summary>
        /// <returns>Returns highlight code list.</returns>
        [ResponseType(typeof(HighlightListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetHighlightCodeList(string attributeCode)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetHighlightCodeList(attributeCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<HighlightListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new HighlightListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #region highlight Product

        /// <summary>
        /// Method to Associate Highlight products.
        /// </summary>
        /// <param name="highlightProductModel">HighlightProductsModel.</param>
        /// <returns>Returns true if product associated successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateAndUnAssociateProduct([FromBody] HighlightProductModel highlightProductModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateAndUnAssociateProduct(highlightProductModel), ErrorCode = 0 });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #endregion
    }
}