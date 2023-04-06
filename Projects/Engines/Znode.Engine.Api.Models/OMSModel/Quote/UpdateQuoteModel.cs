using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class UpdateQuoteModel: BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorQuoteIdRequired)]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorQuoteIdRequired)]
        public int OmsQuoteId { get; set; }

        public int OmsQuoteStateId { get; set; }
        public int UserId { get; set; }
        public int ShippingId { get; set; }

        public int ShippingTypeId { get; set; }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        public decimal ShippingCost { get; set; }
        public DateTime? InHandDate { get; set; }
        public DateTime? QuoteExpirationDate { get; set; }

        public int ShippingAddressId { get; set; }
        public int BillingAddressId { get; set; }

        public decimal TaxCost { get; set; }
        public bool IsTaxExempt { get; set; }
        public decimal SubTotal { get; set; }

        public decimal QuoteTotal { get; set; }

        public List<QuoteLineItemModel> QuoteLineItems { get; set; }

        public string QuoteHistory { get; set; }
        public string AdditionalInstructions { get; set; }
        public string JobName { get; set; }
        public string ShippingConstraintCode { get; set; }
        public bool IsOldQuote { get; set; }
        public decimal ShippingHandlingCharges { get; set; }
        public decimal ImportDuty { get; set; }
        public List<TaxSummaryModel> TaxSummaryList { get; set; }
    }
}
