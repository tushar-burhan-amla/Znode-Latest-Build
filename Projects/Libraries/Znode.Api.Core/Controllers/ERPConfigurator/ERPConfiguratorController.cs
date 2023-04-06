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
    public class ERPConfiguratorController : BaseController
    {
        #region Private Variables
        private readonly IERPConfiguratorCache _cache;
        private readonly IERPConfiguratorService _service;
        #endregion

        #region Constructor
        public ERPConfiguratorController(IERPConfiguratorService service)
        {
            _service = service;
            _cache = new ERPConfiguratorCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets list of ERPConfigurator.
        /// </summary>
        /// <returns>Returns list of ERPConfigurator.</returns>
        [ResponseType(typeof(ERPConfiguratorListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetERPConfiguratorList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ERPConfiguratorListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get all ERP Configurator Classes which are not present in database.
        /// </summary>
        /// <returns>Return ERP configurator classes.</returns>
        [ResponseType(typeof(ERPConfiguratorListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAllERPConfiguratorClassesNotInDatabase()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAllERPConfiguratorClassesNotInDatabase(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ERPConfiguratorListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create ERPConfigurator.
        /// </summary>
        /// <param name="eRPConfiguratorModel">eRPConfiguratorModel model.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(ERPConfiguratorResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] ERPConfiguratorModel eRPConfiguratorModel)
        {
            HttpResponseMessage response;
            try
            {
                ERPConfiguratorModel eRPConfigurator = _service.Create(eRPConfiguratorModel);
                if (HelperUtility.IsNotNull(eRPConfigurator))
                {
                    response = CreateCreatedResponse(new ERPConfiguratorResponse { ERPConfigurator = eRPConfigurator });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(eRPConfigurator.ERPConfiguratorId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get eRPConfigurator by eRPConfigurator id.
        /// </summary>
        /// <param name="eRPConfiguratorId">ERPConfigurator id to get eRPConfigurator details.</param>
        /// <returns>Returns eRPConfigurator model.</returns>
        [ResponseType(typeof(ERPConfiguratorResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetERPConfigurator(int eRPConfiguratorId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetERPConfigurator(eRPConfiguratorId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ERPConfiguratorResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update eRPConfigurator.
        /// </summary>
        /// <param name="eRPConfiguratorModel">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(ERPConfiguratorResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] ERPConfiguratorModel eRPConfiguratorModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update eRPConfigurator.
                bool eRPConfigurator = _service.Update(eRPConfiguratorModel);
                response = eRPConfigurator ? CreateCreatedResponse(new ERPConfiguratorResponse { ERPConfigurator = eRPConfiguratorModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(eRPConfiguratorModel.ERPConfiguratorId)));
            }

            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Delete eRPConfigurator.
        /// </summary>
        /// <param name="eRPConfiguratorId">ERPConfigurator Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel eRPConfiguratorId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.Delete(eRPConfiguratorId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AccountLocked });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Enable disable ERPConfigurator on the basis of eRPConfiguratorId.
        /// </summary>
        /// <param name="eRPConfiguratorId">eRPConfiguratorId to enable disable ERPConfigurator.</param>
        /// <param name="isActive">Enable disable ERPConfigurator on the basis of isActive..</param>
        /// <returns>Returns true/false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage EnableDisableERPConfigurator(string eRPConfiguratorId, bool isActive)
        {
            HttpResponseMessage response;
            try
            {
                bool resp = _service.EnableDisableERPConfigurator(eRPConfiguratorId, isActive);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = resp });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get the class name of active ERP 
        /// </summary>
        /// <returns>Return ERP configurator.</returns>
        [ResponseType(typeof(ERPConfiguratorResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetActiveERPClassName()
        {
            HttpResponseMessage response;
            try
            {
                string data = _service.GetActiveERPClassName();
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse(new ERPConfiguratorResponse { ActiveERPClassName = data }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get the name of ERP class define by User.
        /// </summary>
        /// <returns>Return ERP configurator.</returns>
        [ResponseType(typeof(ERPConfiguratorResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetERPClassName()
        {
            HttpResponseMessage response;
            try
            {
                string data = _service.GetERPClassName();
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse(new ERPConfiguratorResponse { ERPClassName = data }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}
