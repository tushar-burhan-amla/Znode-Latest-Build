namespace Znode.Engine.Api.Models
{
    public class ImpersonationAPIModel : BaseModel
    {
        public string Token { get; set; }
        public int PortalId { get; set; }
        public int CRSUserId { get; set; }
        public string CRSName { get; set; }
        public int WebstoreUserId { get; set; }
        public string ShopperName { get; set; }
        public bool IsImpersonation { get; set; }
        public string UserName { get; set; }
        public bool Result { get; set; }
    }
}
