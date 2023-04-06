using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    //Permission List Model
    public class PermissionListModel : BaseListModel
    {
        public List<PermissionsModel> Permissions { get; set; }
        public PermissionListModel()
        {
            Permissions = new List<PermissionsModel>();
        }
    }
}
