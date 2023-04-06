namespace Znode.Engine.Api.Models
{
    public class OrderShippingModel : BaseModel
    {
        public int ShippingId { get; set; }
        public string ShippingName { get; set; }
        public string ShippingDiscountDescription { get; set; }      
        public string ResponseCode { get; set; } = "0";
        public string ResponseMessage { get; set; }
        public string ShippingCountryCode { get; set; } = string.Empty;
        public decimal ShippingHandlingCharge { get; set; }
        public decimal ShippingDiscount { get; set; }
        public bool IsValidShippingSetting { get; set; }
        public bool ShippingDiscountApplied { get; set; }
        public string ShippingCode { get; set; }
        public string ShippingTypeName { get; set; }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        public int? OmsOrderDetailsId { get; set; }
        public int? UserId { get; set; }
        public int? ShippingTypeId { get; set; }
        public int? ShippingDiscountType { get; set; }
    }
}
