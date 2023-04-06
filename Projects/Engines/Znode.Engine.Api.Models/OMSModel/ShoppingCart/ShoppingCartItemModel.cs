using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Znode.Engine.Api.Models
{
    public class ShoppingCartItemModel : BaseModel
    {
        public string Description { get; set; }
        public decimal ExtendedPrice { get; set; }
        public string ExternalId { get; set; }
        public int ProductId { get; set; }
        public int ParentProductId { get; set; }
        public int ChildProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal AddOnQuantity { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal CustomShippingCost { get; set; }
        public decimal ProductDiscountAmount { get; set; }
        public int ShippingOptionId { get; set; }
        public string SKU { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? CustomUnitPrice { get; set; }
        public bool InsufficientQuantity { get; set; }
        public string CartDescription { get; set; }
        public string CurrencyCode { get; set; }
        public string ImagePath { get; set; }
        public int MediaConfigurationId { get; set; }
        public string ProductName { get; set; }
        public string GroupId { get; set; }
        public string ProductType { get; set; }
        public string ImageMediumPath { get; set; }
        public decimal? MaxQuantity { get; set; }
        public decimal? MinQuantity { get; set; }
        public string AddOnProductSKUs { get; set; }
        public string AutoAddonSKUs { get; set; }
        public string BundleProductSKUs { get; set; }
        public string ConfigurableProductSKUs { get; set; }
        public List<AssociatedProductModel> GroupProducts { get; set; }
        public decimal QuantityOnHand { get; set; }
        public string SeoPageName { get; set; }
        public string ProductCode { get; set; }
        public string TrackingNumber { get; set; }
        public string UOM { get; set; }
        public bool TrackInventory { get; set; }
        public bool AllowBackOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsEditStatus { get; set; }
        public bool IsSendEmail { get; set; }
        public bool ShipSeperately { get; set; }
        public bool IsItemStateChanged { get; set; }
        public int OmsQuoteId { get; set; }
        public int OmsQuoteLineItemId { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public int OmsOrderStatusId { get; set; }
        public string OrderLineItemStatus { get; set; }
        public int? ParentOmsQuoteLineItemId { get; set; }
        public int? ParentOmsSavedcartLineItemId { get; set; }
        public int? OmsSavedcartLineItemId { get; set; }
        public int? OrderLineItemRelationshipTypeId { get; set; }
        public int Sequence { get; set; }
        public int GroupSequence { get; set; }
        public string CustomText { get; set; }
        public string CartAddOnDetails { get; set; }
        public int RmaReasonForReturnId { get; set; }
        public string RmaReasonForReturn { get; set; }
        public string DownloadableProductKey { get; set; }

        //only in Manage Order case this property will get used.
        public int? OmsOrderId { get; set; }

        //In Return Item tax calculation this property will get used.
        public int? AddOnLineItemId { get; set; }


        public PublishProductModel Product { get; set; }

        [IgnoreDataMember]
        public AddressModel ShippingAddress { set; get; }
        public List<OrderShipmentModel> MultipleShipToAddress { get; set; }
        public decimal TaxCost { get; set; }
        public Dictionary<string, object> PersonaliseValuesList { get; set; }
        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }
        public bool IsAllowedTerritories { get; set; } = true;
        public string TaxTransactionNumber { get; set; }
        public int TaxRuleId { get; set; }
        public bool IsShippingReturn { get; set; }
        public decimal? PartialRefundAmount { get; set; }

        public List<PublishAttributeModel> ProductAttributes { get; set; }
        public bool IsProductEdit { get; set; }

        public Dictionary<string, decimal> AdditionalCost { get; set; }
        public string CultureCode { get; set; }
        public List<AssociatedProductModel> AssociatedAddOnProducts { get; set; }
        public string ParentProductSKU { get; set; }
        public string RmaReturnLineItemStatus { get; set; }
        public string ReturnNumber { get; set; }
        public decimal PerQuantityLineItemDiscount { get; set; }
        public decimal PerQuantityCSRDiscount { get; set; }
        public int? ParentOmsOrderLineItemsId { get; set; }
        public decimal PerQuantityShippingCost { get; set; }
        public decimal PerQuantityShippingDiscount { get; set; }
        public decimal PerQuantityOrderLevelDiscountOnLineItem { get; set; }
        public decimal? ProductLevelTax { get; set; }
        public int? PaymentStatusId { get; set; }
        public DateTime? ShipDate { get; set; }
        public decimal PerQuantityVoucherAmount { get; set; }
        public virtual decimal PromotionalPrice { get; set; }
        public decimal DiscountAmount { get; set; }    
        public List<AssociatedPublishedBundleProductModel> BundleProducts { get; set; }
        public string InventoryTracking { get; set; }//map inventory type when this model is bind for exiting order

        public decimal InitialPrice { get; set; }
        public decimal InitialShippingCost { get; set; }
        public bool IsPriceEdit { get; set; }
        public decimal ImportDuty { get; set; }
        public string TemplateType { get; set; }
    }
}
