namespace Znode.Engine.Api.Models
{
    public class OrderShipmentModel : BaseModel
    {
        public int OmsOrderShipmentId { get; set; }
        public string ShipName { get; set; }
        public int? ShippingOptionId { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToCompanyName { get; set; }
        public string ShipToCountry { get; set; }
        public string ShipToEmailId { get; set; }
        public string ShipToFirstName { get; set; }
        public string ShipToLastName { get; set; }
        public string ShipToPhoneNumber { get; set; }
        public string ShipToPostalCode { get; set; }
        public string ShipToStateCode { get; set; }
        public string ShipToStreet1 { get; set; }
        public string ShipToStreet2 { get; set; }

        public int AddressId { get; set; }
        public decimal Quantity { get; set; }
        public decimal TaxCost { get; set; }
        public decimal ShippingCost { get; set; }
        public string ShippingName { get; set; }
    }
}
