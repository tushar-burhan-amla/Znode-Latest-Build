using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class PermissionsMap
    {
        //Convert ZnodeAccessPermission entity in to PermissionsModel model.
        public static PermissionsModel ToModel(ZnodeAccessPermission entity)
        {
            if (!Equals(entity, null))
            {
                return new PermissionsModel
                {
                    PermissionId = entity.AccessPermissionId,
                    PermissionName = entity.PermissionsName,
                    PermissionCode = entity.PermissionCode,
                    IsActive = entity.IsActive,
                    TypeOfPermission= entity.TypeOfPermission,
                };
            }
            else
                return null;
        }

        //Convert IList<ZnodeAccessPermission> in to PermissionListModel model.
        public static PermissionListModel ToListModel(IList<ZnodeAccessPermission> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new PermissionListModel();
                foreach (var item in entity)
                {
                    model.Permissions.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }
    }
}
