using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class RoleMap
    {
        //Convert AspNetRole entity in to RoleModel model.
        public static RoleModel ToModel(AspNetRole entity)
        {
            if (HelperUtility.IsNotNull(entity))
            {
                return new RoleModel
                {
                    RoleId = entity.Id,
                    Name = entity.Name,
                    IsActive = entity.IsActive,
                    IsSystemDefined = entity.IsSystemDefined,
                    TypeOfRole = entity.TypeOfRole
                };
            }
            else
                return null;
        }

           
        //Convert IList<AspNetRole> in to RoleListModel model.
        public static RoleListModel ToListModel(IList<AspNetRole> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new RoleListModel();
                foreach (var item in entity)
                {
                    model.Roles.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }

        //Convert ZnodeRoleMenu entity to RoleMenuModel
        public static RoleMenuModel ToRoleMenuModel(ZnodeRoleMenu entity)
        {
            if (!Equals(entity, null))
            {
                return new RoleMenuModel
                {
                    MenuId = entity.MenuId.Value,
                    RoleMenuId = entity.RoleMenuId,
                    RoleId = entity.RoleId,
                };
            }
            else
                return null;
        }

        //Converts menu id and role id to ZnodeRoleMenu
        public static ZnodeRoleMenu ToRoleMenuEntity(int menuId, string roleId)
        {
            if (HelperUtility.IsNotNull(menuId))
            {
                return new ZnodeRoleMenu
                {
                    MenuId = menuId,
                    RoleId = roleId.ToString(),
                };
            }
            else
                return null;
        }

        //Converts permissionId and roleMenuId to ZnodeRoleMenuAccessMapper
        public static ZnodeRoleMenuAccessMapper ToRolePermissionEntity(int permissionId, int roleMenuId)
        {
            if (HelperUtility.IsNotNull(permissionId))
            {
                return new ZnodeRoleMenuAccessMapper()
                {

                    RoleMenuId = roleMenuId,
                    AccessPermissionId = permissionId,
                };
            }
            else
                return null;
        }

        //Converts menu id and role id to ZnodeRoleMenu
        public static ZnodeRoleMenu ToRoleMenusEntity(RoleMenuModel model)
        {
            if (!Equals(model, null))
            {
                return new ZnodeRoleMenu
                {
                    RoleMenuId = Convert.ToInt32(model.RoleMenuId),
                    MenuId = model.MenuId,
                    RoleId = model.RoleId.ToString(),
                };
            }
            else
                return null;
        }

        //Converts permissionId and roleMenuId to ZnodeRoleMenuAccessMapper
        public static ZnodeRoleMenuAccessMapper ToRoleMenuPermissionEntity(RoleMenuModel model)
        {
            if (!Equals(model, null))
            {
                return new ZnodeRoleMenuAccessMapper()
                {
                    RoleMenuId = Convert.ToInt32(model.RoleMenuId),
                };
            }
            else
                return null;
        }

        //Converts IList<ZnodeRoleMenu> entity to RoleMenuListModel.
        public static RoleMenuListModel ToRoleMenuListModel(IList<ZnodeRoleMenu> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new RoleMenuListModel();
                foreach (var m in entity)
                {
                    model.RoleMenuAccessMapper.AddRange(ToRoleMenuAccessMapperModel(m.ZnodeRoleMenuAccessMappers, m.MenuId));
                }
                return model;
            }
            else
                return null;
        }

        //Converts ICollection<ZnodeRoleMenuAccessMapper> entity to List<RoleMenuAccessMapperModel>
        public static List<RoleMenuAccessMapperModel> ToRoleMenuAccessMapperModel(ICollection<ZnodeRoleMenuAccessMapper> entity, int? menuId)
        {
            if (!Equals(entity, null))
            {
                List<RoleMenuAccessMapperModel> _list = new List<RoleMenuAccessMapperModel>();
                foreach (var item in entity?.OrderBy( x=> x.AccessPermissionId))
                {
                    _list.Add(new RoleMenuAccessMapperModel() { AccessPermissionsId = item.AccessPermissionId, RoleMenuAccessMapperId = item.RoleMenuAccessMapperId, MenuId = Convert.ToInt32(menuId), RoleMenuId = item.RoleMenuId });
                }
                return _list;
            }
            else
                return null;
        }
    }
}
