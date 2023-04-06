using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
namespace Znode.Engine.Api.Cache
{
    public class RoleCache : BaseCache, IRoleCache
    {
        private readonly IRoleService _service;

        //Constructor
        public RoleCache(IRoleService roleService)
        {
            _service = roleService;
        }

        public virtual string GetPermissions(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Permission list
                PermissionListModel list = _service.GetPermissions(Filters);
                if (list?.Permissions?.Count > 0)
                {
                    //Get response and insert it into cache.
                    RoleListResponse response = new RoleListResponse { Permissions = list.Permissions };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetRoles(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Role list
                RoleListModel list = _service.GetRoles(Expands, Filters, Sorts, Page);
                if (list?.Roles?.Count > 0)
                {
                    //Get response and insert it into cache.
                    RoleListResponse response = new RoleListResponse { Roles = list.Roles };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetRole(string roleId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get role by role id.
                RoleModel role = _service.GetRole(roleId);
                if (!Equals(role, null))
                {
                    RoleResponse response = new RoleResponse { Role = role };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        public virtual string GetRolesMenusPermissionsWithRoleMenus(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Role list
                RoleMenuListModel list = _service.GetRolesMenusPermissionsWithRoleMenus(Filters);
                if (list?.Menus?.Count > 0)
                {
                    //Get response and insert it into cache.
                    RoleMenuListResponse response = new RoleMenuListResponse { RoleMenus = list.RoleMenus, AccessMapper = list.RoleMenuAccessMapper, Menus = list.Menus };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetRolePermission(string userName, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Role permission list
                IEnumerable<RolePermissionModel> list = _service.GetPermissionListByUserName(userName);
                if (!list.Equals(null))
                {
                    //Get response and insert it into cache.
                    RolePermissionResponse response = new RolePermissionResponse { RolePermission = list };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}