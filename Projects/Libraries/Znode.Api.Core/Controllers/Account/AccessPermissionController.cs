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
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class AccessPermissionController : BaseController
    {
        #region Private readonly Variables
        private readonly IAccessPermissionService _service;
        private readonly IAccessPermissionCache _cache;
        #endregion

        #region Public Constructor
        public AccessPermissionController(IAccessPermissionService service)
        {
            _service = service;
            _cache = new AccessPermissionCache(_service);
        }
        #endregion

        /// <summary>
        /// Gets account permissions list.
        /// </summary>        
        /// <returns>Returns account permissions list.</returns>
        [ResponseType(typeof(AccessPermissionResponse))]
        [HttpGet]
        public virtual HttpResponseMessage AccountPermissionList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.AccountPermissionList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccessPermissionResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccessPermissionResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Customers.ToString(),TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get account permissions.
        /// </summary>        
        /// <returns>Returns account permissions.</returns>
        [ResponseType(typeof(AccessPermissionResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountPermission()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAccountPermission(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccessPermissionResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccessPermissionResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Customers.ToString(),TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets account permissions list.
        /// </summary>        
        /// <returns>Returns account permissions list.</returns>
        [ResponseType(typeof(AccessPermissionResponse))]
        [HttpGet]
        public virtual HttpResponseMessage AccessPermissionList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.AccessPermissionList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccessPermissionResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccessPermissionResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Customers.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create account permission.
        /// </summary>
        /// <param name="model">model to create.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(AccessPermissionResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateAccountPermission([FromBody] AccessPermissionModel model)
        {
            HttpResponseMessage response;
            try
            {
                AccessPermissionModel accessPermission = _service.CreateAccountPermission(model);
                if (!Equals(accessPermission, null))
                {
                    response = CreateCreatedResponse(new AccessPermissionResponse { AccessPermission = accessPermission });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(accessPermission.AccessPermissionId)));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccessPermissionResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Customers.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccessPermissionResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Update account permissions.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(AccessPermissionResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage UpdateAccountPermission([FromBody] AccessPermissionModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateAccountPermission(model);
                response = isUpdated ? CreateOKResponse(new AccessPermissionResponse { AccessPermission = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.AccessPermissionId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccessPermissionResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccessPermissionResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete account permission.
        /// </summary>
        /// <param name="accountPermissionIds">Ids to delete.</param>
        /// <returns>if deleted successfully returns true else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteAccountPermission(ParameterModel accountPermissionIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteAccountPermission(accountPermissionIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }
    }
}