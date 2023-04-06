using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class AccessPermissionClient : BaseClient, IAccessPermissionClient
    {
        public virtual AccessPermissionListModel AccountPermissionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AccessPermissionEndpoint.AccountPermissionList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AccessPermissionResponse response = GetResourceFromEndpoint<AccessPermissionResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AccessPermissionListModel accountPermissionList = new AccessPermissionListModel { AccountPermissions = response?.AccessPermissionList?.AccountPermissions };
            accountPermissionList.MapPagingDataFromResponse(response);

            return accountPermissionList;
        }

        public virtual AccessPermissionListModel AccessPermissionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AccessPermissionEndpoint.AccessPermissionList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AccessPermissionResponse response = GetResourceFromEndpoint<AccessPermissionResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AccessPermissionListModel accountPermissionList = new AccessPermissionListModel { AccountPermissions = response?.AccessPermissionList?.AccountPermissions };
            accountPermissionList.MapPagingDataFromResponse(response);

            return accountPermissionList;
        }

        public virtual bool DeleteAccountPermission(ParameterModel attributeGroupIds)
        {
            string endpoint = AccessPermissionEndpoint.DeleteAccountPermission();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(attributeGroupIds), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        public virtual AccessPermissionModel CreateAccountPermission(AccessPermissionModel model)
        {
            string endpoint = AccessPermissionEndpoint.CreateAccountPermission();

            ApiStatus status = new ApiStatus();
            AccessPermissionResponse response = PostResourceToEndpoint<AccessPermissionResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AccessPermission;
        }

        public virtual AccessPermissionModel UpdateAccountPermission(AccessPermissionModel model)
        {
            string endpoint = AccessPermissionEndpoint.UpdateAccountPermission();

            ApiStatus status = new ApiStatus();
            AccessPermissionResponse response = PutResourceToEndpoint<AccessPermissionResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.AccessPermission;
        }

        public virtual AccessPermissionModel GetAccountPermission(ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = AccessPermissionEndpoint.GetAccountPermission();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            AccessPermissionResponse response = GetResourceFromEndpoint<AccessPermissionResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            
            return response?.AccessPermission;
        }
    }
}
