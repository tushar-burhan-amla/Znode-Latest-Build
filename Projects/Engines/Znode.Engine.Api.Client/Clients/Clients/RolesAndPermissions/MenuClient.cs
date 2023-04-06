using Newtonsoft.Json;
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
    public class MenuClient : BaseClient, IMenuClient
    {
        public virtual MenuModel CreateMenu(MenuModel model)
        {
            //Get Endpoint.
            string endpoint = MenuEndpoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            MenuResponse response = PostResourceToEndpoint<MenuResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Menu;
        }

        //Get menu list.
        public virtual MenuListModel GetMenuList(FilterCollection filters, SortCollection sorts) => GetMenuList(filters, sorts, null, null);

        public virtual MenuListModel GetMenuList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = MenuEndpoint.GetMenuList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            MenuListResponse response = GetResourceFromEndpoint<MenuListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Menu list
            MenuListModel list = new MenuListModel { ParentMenus = response?.ParentMenus, Menus = response?.Menus };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        public virtual MenuListModel GetMenuListByParentMenuId(ParameterModel model, string preSelectedMenuIds)
        {
            //Get Endpoint.
            string endpoint = MenuEndpoint.GetMenuListByParentMenuId(preSelectedMenuIds);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            MenuListResponse response = PostResourceToEndpoint<MenuListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            //Menu list.
            MenuListModel list = new MenuListModel { Menus = response?.Menus };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        public virtual bool DeleteMenu(ParameterModel menuIds)
        {
            //Get Endpoint.
            string endpoint = MenuEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(menuIds), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.IsSuccess ?? false;
        }

        //Get menu by menu id.
        public virtual MenuModel GetMenu(int menuId) => GetMenu(menuId, null);

        public virtual MenuModel GetMenu(int menuId, ExpandCollection expands)
        {
            //Get Endpoint
            string endpoint = MenuEndpoint.GetMenuByMenuId(menuId);
            endpoint += BuildEndpointQueryString(expands);

            //Get response.
            ApiStatus status = new ApiStatus();
            MenuResponse response = GetResourceFromEndpoint<MenuResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Menu;
        }

        public virtual MenuModel UpdateMenu(int menuId, MenuModel model)
        {
            //Get Endpoint
            string endpoint = MenuEndpoint.Update(menuId);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            MenuResponse response = PutResourceToEndpoint<MenuResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Menu;
        }

        public virtual MenuListModel GetUnSelectedMenus(ParameterModel menuIds)
        {
            //Get Endpoint.
            string endpoint = MenuEndpoint.GetUnSelectedMenus();

            //Get response
            ApiStatus status = new ApiStatus();
            MenuListResponse response = PostResourceToEndpoint<MenuListResponse>(endpoint, JsonConvert.SerializeObject(menuIds), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            //Menu list
            MenuListModel list = new MenuListModel { ParentMenus = response?.ParentMenus, Menus = response?.Menus };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get list of all available and selected Permissions for actions of perticular controller.
        public virtual MenuActionsPermissionModel GetMenuActionsPermissionList(int menuId, ExpandCollection expands)
        {
            //Get Endpoint
            string endpoint = MenuEndpoint.GetMenuActionsPermissionList(menuId);
            endpoint += BuildEndpointQueryString(expands);

            //Get response.
            ApiStatus status = new ApiStatus();
            MenuResponse response = GetResourceFromEndpoint<MenuResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.MenuActionPermission;
        }


        //Update the permission values against action.       
        public virtual MenuActionsPermissionModel UpdateActionPermissions(MenuActionsPermissionModel menuActionsPermissionModel)
        {
            //Get endpoint having api url.
            string endpoint = MenuEndpoint.UpdateActionPermissions();

            ApiStatus status = new ApiStatus();
            MenuResponse response = PostResourceToEndpoint<MenuResponse>(endpoint, JsonConvert.SerializeObject(menuActionsPermissionModel), status);

            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.MenuActionPermission;
        }
    }
}
