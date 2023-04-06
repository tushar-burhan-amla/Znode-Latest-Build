using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class RoleController : BaseController
    {
        #region Private Variables
        private readonly IRoleService _service;
        private readonly IRoleCache _cache;
        #endregion

        #region Default Constructor
        public RoleController(IRoleService service)
        {
            _service = service;
            _cache = new RoleCache(_service);
        }
        #endregion

        /// <summary>
        /// Method to create roles.
        /// </summary>
        /// <param name="model">RoleModel model.</param>
        /// <returns>Returns created role.</returns>
        [ResponseType(typeof(RoleResponse))]
        [HttpPost, ValidateModel]
        public virtual  HttpResponseMessage Create([FromBody] RoleModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create role.
                RoleModel role = _service.CreateRole(model);
                if (!Equals(role, null))
                {
                    response = CreateCreatedResponse(new RoleResponse { Role = role });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(role.RoleId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RoleResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RoleResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of permissions.
        /// </summary>
        /// <returns>Returns permission list.</returns>
        [ResponseType(typeof(RoleListResponse))]
        [HttpGet]
        public virtual  HttpResponseMessage GetPermissionList()
        {
            HttpResponseMessage response;

            try
            {
                //Get permissions.
                string data = _cache.GetPermissions(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RoleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RoleListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of roles.
        /// </summary>
        /// <returns>Returns role list.</returns>
        [ResponseType(typeof(RoleListResponse))]
        [HttpGet]
        public virtual  HttpResponseMessage GetRoleList()
        {
            HttpResponseMessage response;

            try
            {
                //Get roles.
                string data = _cache.GetRoles(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RoleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RoleListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Deletes an existing role
        /// </summary>
        /// <param name="roleIds">Role Id</param>
        /// <returns>Returns deleted role.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual  HttpResponseMessage Delete([FromBody] ParameterModel roleIds)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteRole(roleIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets a role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>Returns role on the basis of role id.</returns>
        [ResponseType(typeof(RoleResponse))]
        [HttpGet]
        public virtual  HttpResponseMessage GetRole(string roleId)
        {
            HttpResponseMessage response;

            try
            {
                //Get role by role id.
                string data = _cache.GetRole(roleId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RoleResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RoleResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Updates an existing role.
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <param name="model">The model of the role.</param>
        /// <returns></returns>
        [ResponseType(typeof(RoleResponse))]
        [HttpPut]
        public virtual  HttpResponseMessage Update(string roleId, [FromBody] RoleModel model)
        {
            HttpResponseMessage response;

            try
            {
                //Update role.
                response = _service.UpdateRole(model) ? CreateOKResponse(new RoleResponse { Role = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RoleResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RoleResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get role menu permissions with role menu.
        /// </summary>
        /// <returns>Returns role menu permissions with role menu.</returns>
        [ResponseType(typeof(RoleMenuListResponse))]
        [HttpGet]
        public virtual  HttpResponseMessage GetRolesMenusPermissionsWithRoleMenus()
        {
            HttpResponseMessage response;

            try
            {
                //Get roles.
                string data = _cache.GetRolesMenusPermissionsWithRoleMenus(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RoleMenuListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RoleMenuListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get Permission List By UserName.
        /// </summary>
        /// <param name="model">User Model</param>
        /// <returns>Return Permission List By UserName.</returns>
        [ResponseType(typeof(RolePermissionResponse))]
        [HttpPost]
        public virtual  HttpResponseMessage GetPermissionListByUserName([FromBody] UserModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Get roles permission list.
                string data = _cache.GetRolePermission(model.UserName, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RolePermissionResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new RolePermissionResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RolePermissionResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }
    }
}