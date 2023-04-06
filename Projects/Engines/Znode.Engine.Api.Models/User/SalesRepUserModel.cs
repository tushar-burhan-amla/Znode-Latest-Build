namespace Znode.Engine.Api.Models
{
    public class SalesRepUserModel : BaseModel
    {
        public int UserId { get; set; }
        public string AspNetUserId { get; set; }
        public string AspNetZnodeUserId { get; set; }
        public string Email { get; set; }
        public string StoreCode { get; set; }
        public string BaseUrl { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public int? PortalId { get; set; }        
    }
}
