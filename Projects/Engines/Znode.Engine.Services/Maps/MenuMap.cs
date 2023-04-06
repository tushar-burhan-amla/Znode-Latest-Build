using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class MenuMap
    {
        //Convert IList<ZnodeMenu> in to MenuListModel model.
        public static MenuListModel ToListModel(IList<ZnodeMenu> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new MenuListModel();
                foreach (var item in entity)
                {
                    model.Menus.Add(item.ToModel<MenuModel>());
                }
                return model;
            }
            else
                return null;
        }

        //Convert ZnodeRoleMenu entity in to RoleMenuModel model.
        public static RoleMenuModel ToRoleMenuModel(ZnodeRoleMenu entity)
        {
            if (!Equals(entity, null))
            {
                return new RoleMenuModel
                {
                    RoleMenuId = entity.RoleMenuId,
                    RoleId = entity.RoleId,
                    MenuId = entity.MenuId.Value,
                };
            }
            else
                return null;
        }

        //Convert IList<ZnodeRoleMenu> in to RoleMenuListModel model.
        public static RoleMenuListModel ToListModel(IList<ZnodeRoleMenu> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new RoleMenuListModel();
                foreach (var item in entity)
                {
                    model.RoleMenus.Add(ToRoleMenuModel(item));
                }
                return model;
            }
            else
                return null;
        }
    }
}
