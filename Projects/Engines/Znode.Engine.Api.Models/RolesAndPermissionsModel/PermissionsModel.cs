namespace Znode.Engine.Api.Models
{
    //Permissions Model
    public class PermissionsModel : BaseModel
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string PermissionCode { get; set; }        
        public string TypeOfPermission { get; set; }
        public bool? IsActive { get; set; }
    }
}
