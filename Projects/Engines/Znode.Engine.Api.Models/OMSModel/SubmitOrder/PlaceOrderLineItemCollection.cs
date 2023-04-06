using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PlaceOrderlineItemCollection : BaseModel
    {
        public string ProductName { get; set; }
        public List<PlaceOrderAttributeModel> OrderAttribute { get; set; }
        public decimal? PartialRefundAmount { get; set; }
        public decimal Price { get; set; }
        public int? OrderLineItemRelationshipTypeId { get; set; }
        public int OmsOrderShipmentId { get; set; }
        public bool? ShipSeparately { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ShippingCost { get; set; }
        public int? OrderLineItemStateId { get; set; }
        public int BundleQuantity { get; set; }
        public bool IsShippingReturn { get; set; }
        public string Sku { get; set; }

        //This property is used for product customization
        public string GroupId { get; set; }
        public decimal Quantity { get; set; }
        public string Description { get; set; }
        public decimal SalesTax { get; set; }
        public decimal VAT { get; set; }
        public decimal GST { get; set; }
        public decimal PST { get; set; }
        public decimal HST { get; set; }
        public decimal ImportDuty { get; set; }
        public string TaxTransactionNumber { get; set; }
        public int TaxRuleId { get; set; }
        public string ParentProductSKU { get; set; }
        public int GroupIdentifier { get; set; }
        public string TrackingNumber { get; set; }
    }
}
