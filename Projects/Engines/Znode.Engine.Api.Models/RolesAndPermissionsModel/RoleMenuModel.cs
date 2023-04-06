using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    //Role Menu Model
    public class RoleMenuModel : BaseModel
    {
        public int? RoleMenuId { get; set; }
        public string RoleId { get; set; }
        public int MenuId { get; set; }
        public List<DataObjectModel> RolePermissions { get; set; }
    }
}
