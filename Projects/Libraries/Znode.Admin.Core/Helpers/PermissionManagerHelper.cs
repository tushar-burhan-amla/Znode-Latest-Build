using System.Collections.Generic;

namespace Znode.Engine.Admin.Helpers
{
    public delegate List<string> PermissionList();
    public static class PermissionManagerHelper
    {
        public static PermissionList UserPermissionList;

        //Check whether the specified permission key is in User Permission list or not.
        public static bool HasUserPermissionRights(string permissionKey)
        {
            permissionKey = string.IsNullOrEmpty(permissionKey) ? string.Empty : permissionKey.ToLower();
            if (UserPermissionList().Contains(permissionKey))
            {
                return true;
            }
            else
            {
                string[] currentControllerName = permissionKey.Split('/');
                return (UserPermissionList().Contains(currentControllerName[0] + "/*")) ? true : false;
            }
        }
    }
}