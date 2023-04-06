using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class QuoteSessionModel : BaseViewModel
    {
        public int OmsQuoteId { get; set; }
        public int OmsQuoteStateId { get; set; }
        public string QuoteStatus { get; set; }
        public int PortalId { get; set; }
        public int LocaleId { get; set; }
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public int PublishCatalogId { get; set; }
        public string CultureCode { get; set; }
        public DateTime? InHandDate { get; set; }
        public DateTime? QuoteExpirationDate { get; set; }

        public AddressModel BillingAddressModel { get; set; }
        public AddressModel ShippingAddressModel { get; set; }

        public decimal SubTotal { get; set; }
        public decimal QuoteTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TaxAmount { get; set; }

        public int ShippingId { get; set; }
        public int ShippingTypeId { get; set; }
        public string ShippingType { get; set; }
        public string ShippingTypeClassName { get; set; }
        public string ShippingDiscountDescription { get; set; }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }

        public Dictionary<string, string> QuoteHistory { get; set; }

        public QuoteCartViewModel CartInformation { get; set; }
        public CustomerInfoViewModel CustomerInformation { get; set; }
        public bool EnableAddressValidation { get; set; }
        public Dictionary<string, QuoteLineItemHistoryModel> QuoteLineItemHistory { get; set; }
        public string JobName { get; set; }
        public string ShippingConstraintCode { get; set; }
        public bool IsOldQuote { get; set; }
        public decimal ImportDuty { get; set; }
        public List<TaxSummaryModel> TaxSummaryList { get; set; }
    }
}
