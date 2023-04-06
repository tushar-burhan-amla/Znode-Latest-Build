using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AccessPermissionListModel : BaseListModel
    {
        public List<AccessPermissionModel> AccountPermissions { get; set; }
    }
}
