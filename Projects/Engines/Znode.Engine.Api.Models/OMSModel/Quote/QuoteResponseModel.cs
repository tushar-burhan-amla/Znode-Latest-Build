using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class QuoteResponseModel : BaseModel
    {
        public int OmsQuoteId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public int OmsQuoteStateId { get; set; }
        public string QuoteNumber { get; set; }
        public string QuoteStatus { get; set; }
        public bool IsConvertedToOrder { get; set; }

        public string CultureCode { get; set; }
        public int LocaleId { get; set; }
        public int PublishCatalogId { get; set; }
        public string StoreName { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? InHandDate { get; set; }
        public DateTime? QuoteExpirationDate { get; set; }

        public int ShippingAddressId { get; set; }
        public int BillingAddressId { get; set; }
        public AddressModel BillingAddressModel { get; set; }
        public AddressModel ShippingAddressModel { get; set; }
        public string BillingAddressHtml { get; set; }
        public string ShippingAddressHtml { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
     
        public decimal QuoteTotal { get; set; }
        public decimal TaxCost { get; set; }
        public decimal SubTotal { get; set; } 
        public decimal ImportDuty { get; set; }
        public decimal ShippingCost { get; set; }
        public bool IsTaxExempt { get; set; }
        public decimal TaxAmount { get; set; }
        public int ShippingId { get; set; }
        public int ShippingTypeId { get; set; }        
        public string ShippingType { get; set; }
        public string ShippingTypeClassName { get; set; }
        public string ShippingDiscountDescription { get; set; }
        
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        public List<ShoppingCartItemModel> ShoppingCartItems { get; set; }
        public string CartItemCount { get; set; }

        public List<OrderHistoryModel> QuoteHistoryList { get; set; }

        public string ShippingConstraintCode { get; set; }
        public string JobName { get; set; }
        public decimal ShippingHandlingCharges { get; set; }
        public bool IsOldQuote { get; set; }
        public List<TaxSummaryModel> TaxSummaryList { get; set; }
        public int OmsOrderId { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerGUID { get; set; }
    }
}