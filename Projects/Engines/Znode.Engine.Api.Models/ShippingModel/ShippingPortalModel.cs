namespace Znode.Engine.Api.Models
{
    public class ShippingPortalModel : BaseModel
    {
        public int ShippingPortalId { get; set; }
        public int ShippingId { get; set; }
        public int PortalId { get; set; }
        public string StoreName { get; set; }
    }
}
