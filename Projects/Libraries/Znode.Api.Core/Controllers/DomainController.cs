using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Exceptions;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class DomainController : BaseController
    {
        #region Private Members
        private readonly IDomainService _service;
        private readonly IDomainCache _cache;
        #endregion

        #region Controller
        public DomainController(IDomainService service)
        {
            _service = service;
            _cache = new DomainCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a list of Domain.
        /// </summary>
        /// <returns>List of Domain</returns> 
        [ResponseType(typeof(DomainListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDomains(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DomainListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new DomainListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get Domain by domainId
        /// </summary>
        /// <param name="domainId">DomainId to get domain</param>
        /// <returns>response</returns>
        [ResponseType(typeof(DomainResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int domainId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDomain(domainId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DomainResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                  ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new DomainResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Create new Domain
        /// </summary>
        /// <param name="domainModel">DomainModel</param>
        /// <returns>Response</returns>
        [ResponseType(typeof(DomainResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody]DomainModel domainModel)
        {
            HttpResponseMessage response;
            try
            {
                DomainModel domain = _service.CreateDomain(domainModel);
                if (!Equals(domain, null))
                {
                    response = CreateCreatedResponse(new DomainResponse { Domain = domain });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(domain.DomainId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new DomainResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DomainResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        ///  Update Domain
        /// </summary>
        /// <param name="domainModel">DomainModel</param>
        /// <returns>Response</returns>
        [ResponseType(typeof(DomainResponse))]
        [HttpPut]
        [ValidateModel]
        public virtual HttpResponseMessage Update([FromBody]DomainModel domainModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateDomain(domainModel);
                response = isUpdated ? CreateOKResponse(new DomainResponse { Domain = domainModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new DomainResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DomainResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Delete Domain
        /// </summary>
        /// <param name="domainIds">domainIds to delete domain</param>
        /// <returns>Response</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Delete(ParameterModel domainIds)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteDomain(domainIds) });
            } catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            } catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Enable disable domain.
        /// </summary>
        /// <param name="domainModel">domain Model.</param>
        /// <returns>Returns response.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage EnableDisableDomain([FromBody] DomainModel domainModel)
        {
            HttpResponseMessage response;

            try
            {
                bool resp = _service.EnableDisableDomain(domainModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = resp });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message});
            }

            return response;
        }
        #endregion
    }
}
