using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class QuoteResponseViewModel : BaseViewModel
    {
        public int OmsQuoteId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public int OmsQuoteStateId { get; set; }
        public string QuoteNumber { get; set; }    
        public string QuoteStatus { get; set; }
        public bool IsConvertedToOrder { get; set; }
        public string CreatedByName { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
        public int LocaleId { get; set; }
        public DateTime? QuoteExpirationDate { get; set; }
        public int ShippingAddressId { get; set; }
        public int BillingAddressId { get; set; }
        public AddressModel BillingAddressModel { get; set; }
        public AddressModel ShippingAddressModel { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? InHandDate { get; set; }
        public decimal QuoteTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TaxCost { get; set; }
        public decimal ImportDuty { get; set; }
        public string CartItemCount { get; set; }
        public string ShippingAddressHtml { get; set; }
        public string BillingAddressHtml { get; set; }
        public string ShippingType { get; set; }
        public decimal SubTotal { get; set; }
        public List<ShoppingCartItemModel> ShoppingCartItems { get; set; }
        public string ShippingConstraintCode { get; set; }
        public string JobName { get; set; }
        public bool EnableConvertToOrder { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<QuoteNotesViewModel> QuoteNoteList { get; set; }
        public decimal ShippingHandlingCharges { get; set; }
        public string CustomerPaymentGUID { get; set; }
        public List<TaxSummaryViewModel> TaxSummaryList { get; set; }
    }
}
