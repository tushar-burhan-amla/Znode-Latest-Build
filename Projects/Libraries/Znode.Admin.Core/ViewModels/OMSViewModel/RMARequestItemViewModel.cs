using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class RMARequestItemViewModel : BaseViewModel
    {
        public int? OmsOrderLineItemsId { get; set; }
        public int OMSOrderId { get; set; }
        public string ProductNum { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal? MaxQuantity { get; set; }
        public decimal? Price { get; set; }
        public string SKU { get; set; }
        public string DiscountAmount { get; set; }
        public string OrderNumber { get; set; }
        public string ShippingCost { get; set; }
        public string PromoDescription { get; set; }
        public string SalesTax { get; set; }
        public decimal? RMAMaxQuantity { get; set; }
        public decimal RMAQuantity { get; set; }
        public bool? IsReturnable { get; set; }
        public bool? IsReceived { get; set; }
        public int? RmaReasonForReturnId { get; set; }
        public decimal? TaxCost { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public string TaxCostString { get; set; }
        public string SubTotalString { get; set; }
        public string TotalString { get; set; }
        public string ReasonForReturn { get; set; }
        public int RowCount { get; set; }
        public IEnumerable<SelectListItem> RmaQuantitySelectList { get; set; }
        public IEnumerable<SelectListItem> ReasonForReturnList { get; set; }
        public int RMARequestItemID { get; set; }
        public int AccountID { get; set; }
        public int? GCExpirationPeriod { get; set; }
        public decimal? Quantity { get; set; }
        public int? GiftCardId { get; set; }
        public string PriceWithCurrencySymbol { get; set; }
        public int RMArequestId { get; set; }
        public string TransactionId { get; set; }
        public bool QuantityDropDownEnabled { get; set; }
        public bool IsReturnableCheckboxEnabled { get; set; }
        public decimal TotalPrice { get; set; }
        public string ReasonforReturn { get; set; }
        public decimal? RmaQuantitySelect { get; set; }
        public decimal UnitPrice { get; set; }
        public string CurrencyCode { get; set; }
        public string UnitPriceWithCurrencySymbol { get; set; }
        public int? UserId { get; set; }
        public string CultureCode { get; set; }
    }
}