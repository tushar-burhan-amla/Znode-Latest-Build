using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ReturnOrderLineItemViewModel : BaseViewModel
    {
        public string PublishProductId { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }

        public string ProductType { get; set; }
        public string GroupProductsQuantity { get; set; }
        public string SKU { get; set; }
        public int OrderLineItemStatusId { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public string OrderLineItemStatus { get; set; }
        public string TrackingNumber { get; set; }

        public string ImagePath { get; set; }
        public string CurrencyCode { get; set; }
        public string ExtendedPrice { get; set; }
        public string UnitPrice { get; set; }
        public string Total { get; set; }
        public string ShippingCost { get; set; }
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
        public string GroupProductSKUs { get; set; }
        public string ProductCode { get; set; }
        public List<AssociatedProductModel> GroupProducts { get; set; }
        public List<SelectListItem> ShippingStatusList { get; set; }
        public string PersonalisedCodes { get; set; }
        public string PersonalisedValues { get; set; }
        public Dictionary<string, object> PersonaliseValuesList { get; set; }
        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }
        public string ExternalId { get; set; }
        public bool InsufficientQuantity { get; set; }
        public bool IsReviewPage { get; set; }
        public bool IsEditStatus { get; set; }
        public bool IsSendEmail { get; set; }
        public bool ShipSeperately { get; set; }
        public int ReasonForReturnId { get; set; }
        public decimal? MaxQuantity { get; set; }
        public decimal? MinQuantity { get; set; }
        public string UOM { get; set; }
        public string ReasonForReturn { get; set; }

        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public bool IsShippingReturn { get; set; }
        public string CultureCode { get; set; }
        public string RmaReturnLineItemStatus { get; set; }
        public string ReturnNumber { get; set; }
    }
}
