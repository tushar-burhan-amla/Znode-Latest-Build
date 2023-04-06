namespace Znode.Engine.Api.Models
{
    public class AddToCartStatusModel : BaseModel
    {       
        public bool Status { get; set; }
        public int SavedCartId { get; set; }
        public string CookieMapping { get; set; }
        public int CookieMappingId { get; set; }
        public decimal CartCount { get; set; }
    }
}
