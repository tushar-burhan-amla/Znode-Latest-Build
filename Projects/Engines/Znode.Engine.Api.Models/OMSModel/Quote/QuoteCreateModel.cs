using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class QuoteCreateModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorUserIdRequired)]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorUserIdRequired)]
        public int UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorPortalIdRequired)]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorPortalIdRequired)]
        public int PortalId { get; set; }

        public string OmsQuoteStatus { get; set; }
        public int OmsOrderStateId { get; set; }
        public string QuoteNumber { get; set; }
        public string QuoteTypeCode { get; set; }

        public decimal TaxCost { get; set; }
        public decimal ImportDuty { get; set; }
        public decimal ShippingCost { get; set; }

        public int PublishStateId { get; set; }
        public string AdditionalInstruction { get; set; }

        public DateTime? InHandDate { get; set; }
        public DateTime? QuoteExpirationDate { get; set; }

        //ShippingDetails
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorShippingIdRequired)]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorShippingIdRequired)]
        public int ShippingId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorShippingAddressIdRequired)]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorShippingAddressIdRequired)]
        public int ShippingAddressId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorBillingAddressIdRequired)]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorBillingAddressIdRequired)]
        public int BillingAddressId { get; set; }
        public bool FreeShipping { get; set; }
        public string ShippingCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorQuoteTotalRequired)]
        [RegularExpression(@"^(0*[1-9][0-9]*(\.[0-9]+)?|0+\.[0-9]*[1-9][0-9]*)$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorQuoteTotalRequired)]
        public decimal? QuoteTotal { get; set; }
        public string CookieMappingId { get; set; }

        public int OmsQuoteId { get; set; }

        public string ShippingConstraintCode { get; set; }
        public string JobName { get; set; }

        public List<ProductDetailModel> productDetails { get; set; }
        public bool IsTaxExempt { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal ShippingHandlingCharges { get; set; }
        public List<TaxSummaryModel> TaxSummaryList { get; set; }
        public string ShippingMethod { get; set; }
        public string AccountNumber { get; set; }

        // To save and get the data against AccountId for user account association
        public int AccountId { get; set; }
     }
}
