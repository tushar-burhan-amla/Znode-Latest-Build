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

namespace Znode.Engine.Api.Controllers
{
    public class WebStoreCaseRequestController : BaseController
    {
        #region Private Variables

        private readonly IWebStoreCaseRequestService _service;
        private readonly IWebStoreCaseRequestCache _cache;

        #endregion

        #region Constructor
        public WebStoreCaseRequestController(IWebStoreCaseRequestService service)
        {
            _service = service;
            _cache = new WebStoreCaseRequestCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create case request for contact us page.
        /// </summary>
        /// <param name="CaseRequestModel">Model for case request.</param>
        /// <returns>Returns created case request model for contact us.</returns>
        [ResponseType(typeof(WebStoreCaseRequestResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateContactUs([FromBody] WebStoreCaseRequestModel CaseRequestModel)
        {
            HttpResponseMessage response;

            try
            {
                WebStoreCaseRequestModel CaseRequest = _service.CreateContactUs(CaseRequestModel);

                if (HelperUtility.IsNotNull(CaseRequest))
                    response = CreateCreatedResponse(new WebStoreCaseRequestResponse { CaseRequest = CaseRequest });
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get list of case request.
        /// </summary>
        /// <returns>Returns list of case request.</returns>
        [ResponseType(typeof(WebStoreCaseRequestListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCaseRequests(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Create case request.
        /// </summary>
        /// <param name="webStoreCaseRequestModel">Model for case request.</param>
        /// <returns>Returns created case request model.</returns>
        [ResponseType(typeof(WebStoreCaseRequestResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateCaseRequest([FromBody] WebStoreCaseRequestModel webStoreCaseRequestModel)
        {
            HttpResponseMessage response;
            try
            {
                WebStoreCaseRequestModel caseRequest = _service.CreateCaseRequest(webStoreCaseRequestModel);

                if (HelperUtility.IsNotNull(caseRequest))
                {
                    response = CreateCreatedResponse(new WebStoreCaseRequestResponse { CaseRequest = caseRequest });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(caseRequest.CaseRequestId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get case request data on the basis of case request id.
        /// </summary>
        /// <param name="caseRequestId">Case request id to get case request details.</param>
        /// <returns>Returns case requests details.</returns>
        [ResponseType(typeof(WebStoreCaseRequestResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCaseRequest(int caseRequestId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCaseRequest(caseRequestId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update case request data.
        /// </summary>
        /// <param name="webStoreCaseRequestModel">Model to update case request data.</param>
        /// <returns>Returns updated case request model.</returns>
        [ResponseType(typeof(WebStoreCaseRequestResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateCaseRequest([FromBody] WebStoreCaseRequestModel webStoreCaseRequestModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update case request.
                response = _service.UpdateCaseRequest(webStoreCaseRequestModel) ? CreateCreatedResponse(new WebStoreCaseRequestResponse { CaseRequest = webStoreCaseRequestModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(webStoreCaseRequestModel.CaseRequestId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Send mail to reply to a customer.
        /// </summary>
        /// <param name="webStoreCaseRequestModel">Webstore case request model.</param>
        /// <returns>Returns true if mail sent successfully else return false.</returns>
        [ResponseType(typeof(WebStoreCaseRequestResponse))]
        [HttpPut]
        public virtual HttpResponseMessage ReplyCustomer([FromBody]  WebStoreCaseRequestModel webStoreCaseRequestModel)
        {
            HttpResponseMessage response;

            try
            {
                response = _service.ReplyCustomer(webStoreCaseRequestModel) ? CreateCreatedResponse(new WebStoreCaseRequestResponse { CaseRequest = webStoreCaseRequestModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(webStoreCaseRequestModel.CaseRequestId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Get list of case types.
        /// </summary>
        /// <returns>Returns list of case types.</returns>
        [ResponseType(typeof(CaseTypeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCaseTypeList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCaseTypeList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new CaseTypeListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get list of case request priorities.
        /// </summary>
        /// <returns>Returns list of case request priorities.</returns>
        [ResponseType(typeof(CasePriorityListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCasePriorityList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCasePriorityList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new CasePriorityListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get list of case request status.
        /// </summary>
        /// <returns>Returns list of case request status.</returns>
        [ResponseType(typeof(CaseStatusListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCaseStatusList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCaseStatusList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new CaseStatusListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
    }
}
