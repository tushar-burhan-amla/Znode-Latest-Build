using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    //Role List Model
    public class RoleListModel : BaseListModel
    {
        public List<RoleModel> Roles { get; set; }
        public RoleListModel()
        {
            Roles = new List<RoleModel>();
        }
    }
}
