namespace Znode.Engine.Api.Models
{
    public class AccountPermissionAccessModel : BaseModel
    {
        public int AccountPermissionAccessId { get; set; }
        public int AccountPermissionId { get; set; }
        public int AccessPermissionId { get; set; }
    }
}
