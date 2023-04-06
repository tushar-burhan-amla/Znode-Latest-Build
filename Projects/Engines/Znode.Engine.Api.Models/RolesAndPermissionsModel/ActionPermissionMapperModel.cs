namespace Znode.Engine.Api.Models
{
    public class ActionPermissionMapperModel : BaseModel
    {
        public int AccessPermissionId { get; set; }
        public int ActionId { get; set; }
        public int MenuId { get; set; }
    }
}
 