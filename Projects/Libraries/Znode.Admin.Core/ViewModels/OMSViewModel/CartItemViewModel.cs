using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CartItemViewModel : BaseViewModel
    {
        public CartItemViewModel()
        {
            ExternalId = System.Guid.NewGuid().ToString();
        }

        public string PublishProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal CustomQuantity { get; set; }
        public int ParentProductId { get; set; }
        public string ProductId { get; set; }
        public string ProductType { get; set; }
        public string GroupProductsQuantity { get; set; }
        public string SKU { get; set; }
        public List<PublishAttributeModel> ProductAttributes { get; set; }
        public string GroupId { get; set; }

        public int Sequence { get; set; }
        public int GroupSequence { get; set; }
        public int OrderLineItemStatusId { get; set; }
        public string OrderLineItemStatus { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public string TrackingNumber { get; set; }

        public string ImagePath { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExtendedPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Total { get; set; }
        public decimal? ShippingCost { get; set; }
        public int PortalId { get; set; }
        public int OmsOrderId { get; set; }
        public int CatalogId { get; set; }
        public int UserId { get; set; }
        public int LocaleId { get; set; }
        public int ShippingId { get; set; }
        public string CartDescription { get; set; }
        public string ConfigurableProductSKUs { get; set; }
        public string BundleProductSKUs { get; set; }
        public string AddOnProductSKUs { get; set; }
        public string AutoAddonSKUs { get; set; }
        public string GroupProductSKUs { get; set; }
        public string ProductCode { get; set; }
        public List<AssociatedProductModel> GroupProducts { get; set; }
        public List<SelectListItem> ShippingStatusList { get; set; }
        public string PersonalisedCodes { get; set; }
        public string PersonalisedValues { get; set; }
        public Dictionary<string, object> PersonaliseValuesList { get; set; }
        public string ExternalId { get; set; }
        public bool InsufficientQuantity { get; set; }
        public bool IsReviewPage { get; set; }
        public bool IsEditStatus { get; set; }
        public bool IsSendEmail { get; set; }
        public bool ShipSeperately { get; set; }
        public int? OmsSavedcartLineItemId { get; set; }
        public decimal? MaxQuantity { get; set; }
        public decimal? MinQuantity { get; set; }
        public string UOM { get; set; }
        public string TrackingUrl { get; set; }
        public string GroupProductNames { get; set; }
        public bool IsAllowedTerritories { get; set; }
        public bool IsActive { get; set; }
        public string DownloadableProductKey { get; set; }

        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public decimal TaxCost { get; set; }
        public decimal? PartialRefundAmount { get; set; }
        public int? ParentOmsSavedcartLineItemId { get; set; }
        public bool IsOrderPartialRefund { get; set; }
        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }
        public Dictionary<string, decimal> AdditionalCost { get; set; }
        public string CultureCode { get; set; }
        public decimal InitialPrice { get; set; }
        public bool IsPriceEdit { get; set; }
        public decimal QuantityOnHand { get; set; }
        public decimal? CustomUnitPrice { get; set; }
    }
}