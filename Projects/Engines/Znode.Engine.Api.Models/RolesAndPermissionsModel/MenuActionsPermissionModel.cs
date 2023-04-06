using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class MenuActionsPermissionModel : BaseModel
    {
        public List<ActionModel> ActionList { get; set; }
        public List<ActionPermissionMapperModel> ActionPermissionList { get; set; }
        public int MenuId { get; set; }
        public int ActionId { get; set; }
        public int AccessPermissionId { get; set; }
    }
}
