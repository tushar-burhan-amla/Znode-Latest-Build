namespace Znode.Engine.Api.Models
{
    public class RolePermissionModel: BaseModel
    {
        public int AccessPermissionsId { get; set; }
        public int? MenuId { get; set; }
        public string RequestUrlTemplate { get; set; }
    }
}
