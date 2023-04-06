using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ReturnOrderLineItemModel : BaseModel
    {
        public string Description { get; set; }
        public decimal ExtendedPrice { get; set; }
        public int ProductId { get; set; }
        public int ChildProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal ProductDiscountAmount { get; set; }
        public int ShippingOptionId { get; set; }
        public string SKU { get; set; }
        public string ExternalId { get; set; }

        public decimal UnitPrice { get; set; }
        public string CartDescription { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }        
        public string ImagePath { get; set; }
        public int MediaConfigurationId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ImageMediumPath { get; set; }
        public string AddOnProductSKUs { get; set; }
        public string BundleProductSKUs { get; set; }
        public string ConfigurableProductSKUs { get; set; }
        public List<AssociatedProductModel> GroupProducts { get; set; }
        public string ProductCode { get; set; }
        public string TrackingNumber { get; set; }
        public string UOM { get; set; }
        public bool IsEditStatus { get; set; }
        public bool IsActive { get; set; }
        public bool ShipSeperately { get; set; }
        public decimal? Vat { get; set; }
        public decimal? Gst { get; set; }
        public decimal? Hst { get; set; }
        public decimal? Pst { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? ImportDuty { get; set; }
        public int OmsOrderStatusId { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public int OmsOrderShipmentId { get; set; }
        public int ReasonForReturnId { get; set; }
        public string OrderLineItemStatus { get; set; }
        public string CustomText { get; set; }

        public decimal TaxCost { get; set; }
        public Dictionary<string, object> PersonaliseValuesList { get; set; }
        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }
        public string ReasonForReturn { get; set; }
        public int OrderDetailId { get; set; }
        public int? ParentOmsOrderLineItemsId { get; set; }
        public int? OrderLineItemRelationshipTypeId { get; set; }

        //In Return Item tax calculation this property will get used.
        public int? AddOnLineItemId { get; set; }
        public bool IsAlreadyReturned { get; set; }

        public bool IsShippingReturn { get; set; }
        public decimal? PartialRefundAmount { get; set; }
        public Dictionary<string, decimal> AdditionalCost { get; set; }
        public string RmaReturnLineItemStatus { get; set; }
        public string ReturnNumber { get; set; }
        public decimal PerQuantityLineItemDiscount { get; set; }
        public decimal PerQuantityCSRDiscount { get; set; }
        public decimal PerQuantityShippingCost { get; set; }
        public decimal PerQuantityShippingDiscount { get; set; }
        public decimal PerQuantityOrderLevelDiscountOnLineItem { get; set; }
        public DateTime? ShipDate { get; set; }
        public int PaymentStatusId { get; set; }
        public decimal PerQuantityVoucherAmount { get; set; }
    }
}
