
namespace Znode.Engine.Api.Models
{
    public class PermissionCodeModel : BaseModel
    {
        public int? AccountUserPermissionId { get; set; }
        public int? AccountPermissionAccessId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionsName { get; set; }
        public int UserId { get; set; }
    }
}
