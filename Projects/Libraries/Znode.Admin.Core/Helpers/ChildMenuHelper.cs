using System;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Helpers
{
    [Obsolete("Not Used")]
    public class ChildMenuHelper
    {
        /// <summary>
        /// To Get the Menu list by controller name.
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="localPath"></param>
        /// <returns>Return the List of Menu in MenuListModel format.</returns>
        public MenuListViewModel GetMenuListByControllerName(string controllerName, string areaName)
        {
            try
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeMenuEnum.IsActive.ToString(), FilterOperators.Equals, "true"));

                IMenuAgent _menuAgent = new MenuAgent(new MenuClient(), new RoleClient(), new AccessPermissionClient());

                //Get list of active menu.
                MenuListViewModel menuList = _menuAgent.GetMenuList(filters, null, null, null);
                var lstParentIds = menuList?.Menus?.FindAll(r => r.ControllerName == controllerName)?.Select(x => x.ParentMenuId);

                //Get Parentmenu Id by passing controller name.
                int parentMenuId = Convert.ToInt32(lstParentIds?.Where(x => Convert.ToInt32(x) > 0)?.FirstOrDefault());

                //Get all menus as per the parent menu id by controller name.
                menuList.Menus = menuList?.Menus?.Where(x => x.ParentMenuId == parentMenuId).ToList();

                //If Area name is not null then We split area name & pass only controller name.
                menuList.Menus = string.IsNullOrEmpty(areaName) ? menuList.Menus : menuList?.Menus?.Select(c => { c.ControllerName = c.ControllerName.Split('/')[1]; c.AreaName = areaName; return c; }).ToList();

                return menuList;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
        }
    }
}