using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class MenuCache : BaseCache, IMenuCache
    {
        private readonly IMenuService _service;

        //Constructor
        public MenuCache(IMenuService menuService)
        {
            _service = menuService;
        }

        public virtual string GetMenus(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Menu list
                MenuListModel list = _service.GetMenuList(Expands, Filters, Sorts, Page);
                if (list?.Menus?.Count > 0)
                {
                    //Get response and insert it into cache.
                    MenuListResponse response = new MenuListResponse { ParentMenus = list.ParentMenus, Menus = list.Menus };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetMenu(int menuId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Menu list by menu id.
                MenuModel menu = _service.GetMenu(menuId);
                if (!Equals(menu, null))
                {
                    MenuResponse response = new MenuResponse { Menu = menu };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetMenuActionsPermissionList(int menuId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Menu list by menu id.
                MenuActionsPermissionModel menuActionPermission = _service.GetMenuActionsPermissionList(menuId, Expands);
                if (!Equals(menuActionPermission, null))
                {
                    MenuResponse response = new MenuResponse { MenuActionPermission = menuActionPermission };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}