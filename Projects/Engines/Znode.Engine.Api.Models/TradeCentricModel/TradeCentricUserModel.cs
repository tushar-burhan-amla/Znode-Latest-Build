namespace Znode.Engine.Api.Models
{
    public class TradeCentricUserModel : BaseModel
    {
        public int TradeCentricUserId { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string ReturnUrl { get; set; }
        public int UserId { get; set; }
        public string RedirectUrl { get; set; }
        public string Operation { get; set; }
        public string CookieMappingKey { get; set; }
        public TradeCentricCartModel CartModel { get; set; }

    }
}
