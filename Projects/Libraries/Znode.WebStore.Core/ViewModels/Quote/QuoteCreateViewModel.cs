using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Api.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class QuoteCreateViewModel : BaseViewModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int PortalId { get; set; }
        public string OmsQuoteStatus { get; set; }
        public int OmsQuoteTypeId { get; set; }
        public string QuoteTypeCode { get; set; }
        public int OmsQuoteId { get; set; }

        public decimal TaxCost { get; set; }
        public decimal ImportDuty { get; set; }
        public decimal ShippingCost { get; set; }

        public int PublishStateId { get; set; }
        public string AdditionalInstruction { get; set; }

        public DateTime? InHandDate { get; set; }
        public DateTime? QuoteExpirationDate { get; set; }

        //ShippingDetails
        [Required]
        public int ShippingId { get; set; }
        [Required]
        public int ShippingAddressId { get; set; }
        [Required]
        public int BillingAddressId { get; set; }
        public bool FreeShipping { get; set; }
        public string ShippingCode { get; set; }

        [Required]
        public decimal? QuoteTotal { get; set; }
        public string CookieMappingId { get; set; }
        public string ShippingConstraintCode { get; set; }
        public string JobName { get; set; }
        public List<ProductDetailModel> productDetails { get; set; }
        public decimal ShippingHandlingCharges { get; set; }
        public List<TaxSummaryModel> TaxSummaryList { get; set; }
        public string ShippingMethod { get; set; }
        public string AccountNumber { get; set; }
    }
}
