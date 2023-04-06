namespace Znode.Engine.Api.Models
{
    //Role Menu Access Mapper Model
    public class RoleMenuAccessMapperModel : BaseModel
    {
        public int RoleMenuAccessMapperId { get; set; }
        public int AccessPermissionsId { get; set; }
        public int RoleMenuId { get; set; }
        public int MenuId { get; set; }
    }
}
