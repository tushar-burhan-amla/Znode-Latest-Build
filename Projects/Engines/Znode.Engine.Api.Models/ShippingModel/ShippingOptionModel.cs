namespace Znode.Engine.Api.Models
{
    public class ShippingOptionModel : BaseModel
    {
        public int ShippingId { get; set; }
        public string ShippingName { get; set; }
        public decimal? ShippingRate { get; set; }
        public decimal? ShippingRateWithoutDiscount { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
        public string ShippingCode { get; set; }
        public decimal HandlingCharge { get; set; }
    }
}
