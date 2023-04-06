namespace Znode.Engine.Api.Models
{
    public class OrderShipmentDataModel : BaseModel
    {
        public string AddressId { get; set; }
        public string ProductId { get; set; }
        public decimal Quantity { get; set; }
        public string Slno { get; set; }
        public string ItemGUID{ get; set; }
        public string ShippingDescription { get; set; }
        public int ShippingId { get; set; }
        public string ShippingCode { get; set; }
        public string SKU { get; set; }
        public string AddOnValuesCustomText { get; set; }
        public string AddOnValueIds { get; set; }
        public string CartAddOnDetails { get; set; }
    }
}
