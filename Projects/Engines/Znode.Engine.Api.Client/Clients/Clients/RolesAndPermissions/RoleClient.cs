using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class RoleClient : BaseClient, IRoleClient
    {
        public virtual RoleModel CreateRole(RoleModel model)
        {
            //Get Endpoint
            string endpoint = RoleEndpoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            RoleResponse response = PostResourceToEndpoint<RoleResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Role;
        }

        //Get permission list.
        public virtual PermissionListModel GetPermissionList(FilterCollection filters, SortCollection sorts) => GetPermissionList(filters, sorts, null, null);

        public virtual PermissionListModel GetPermissionList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = RoleEndpoint.GetPermissionList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            RoleListResponse response = GetResourceFromEndpoint<RoleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Permission list.
            PermissionListModel list = new PermissionListModel { Permissions = response?.Permissions };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get role list
        public virtual RoleListModel GetRoleList(FilterCollection filters, SortCollection sorts) => GetRoleList(filters, sorts, null, null);

        public virtual RoleListModel GetRoleList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = RoleEndpoint.GetRoleList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            RoleListResponse response = GetResourceFromEndpoint<RoleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Role list.
            RoleListModel list = new RoleListModel { Roles = response?.Roles };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        public virtual bool DeleteRole(ParameterModel roleIds)
        {
            //Get Endpoint.
            string endpoint = RoleEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(roleIds), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.IsSuccess ?? false;
        }

        //Get role by role id.
        public virtual RoleModel GetRole(string roleId) => GetRole(roleId, null);

        public virtual RoleModel GetRole(string roleId, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = RoleEndpoint.GetRoleByRoleId(roleId);
            endpoint += BuildEndpointQueryString(expands);

            //Get response.
            ApiStatus status = new ApiStatus();
            RoleResponse response = GetResourceFromEndpoint<RoleResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Role;
        }

        public virtual RoleModel UpdateRole(string roleId, RoleModel model)
        {
            //Get Endpoint.
            string endpoint = RoleEndpoint.Update(roleId);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            RoleResponse response = PutResourceToEndpoint<RoleResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Role;
        }

        public virtual RoleMenuListModel GetRolesMenusPermissionsWithRoleMenus(FilterCollection filters)
        {
            //Get Endpoint.
            string endpoint = RoleEndpoint.GetRolesMenusPermissionsWithRoleMenus();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            //Get response.
            ApiStatus status = new ApiStatus();
            RoleMenuListResponse response = GetResourceFromEndpoint<RoleMenuListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //RoleMenus, Permissions and Menus list.
            RoleMenuListModel list = new RoleMenuListModel { RoleMenus = response?.RoleMenus, Menus = response?.Menus, RoleMenuAccessMapper = response?.AccessMapper };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        public virtual IEnumerable<RolePermissionModel> GetRolePermissionByUserName(string userName)
        {
            RefreshCache = false;
            //Get Endpoint.
            string endpoint = RoleEndpoint.GetRolePermissionByUserName();

            ApiStatus status = new ApiStatus();
            UserModel model = new UserModel();
            model.UserName = userName;

            //Get response.
            RolePermissionResponse response = PostResourceToEndpoint<RolePermissionResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.RolePermission ?? new List<RolePermissionModel>();
        }
    }
}
