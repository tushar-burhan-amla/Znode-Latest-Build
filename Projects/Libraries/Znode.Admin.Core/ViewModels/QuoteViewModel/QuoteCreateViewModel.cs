using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class QuoteCreateViewModel : BaseViewModel
    {
        [Display(Name = ZnodeAdmin_Resources.LabelStore, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TitleCatalog, ResourceType = typeof(Admin_Resources))]
        public int CatalogId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAccounts, ResourceType = typeof(Admin_Resources))]
        public int AccountId { get; set; }

        public List<SelectListItem> PortalList { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCustomerName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.CustomerNameRequiredError)]
        public string CustomerName { get; set; }
        public int UserId { get; set; }

        public UserAddressDataViewModel UserAddressDataViewModel { get; set; }

        public CartViewModel CartViewModel { get; set; }

        public ShippingListViewModel ShippingListViewModel { get; set; }

        public int? ShippingId { get; set; }
        public int BillingAddressId { get; set; }
        public int ShippingAddressId { get; set; }
        public int QuoteId { get; set; }
        public string StoreName { get; set; }
        public bool EnableAddressValidation { get; set; }
        public bool IsQuote { get; set; }
        public string AdditionalInstruction { get; set; }
        public bool IsFromUserCart { get; set; }
        public bool IsTaxExempt { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelInHandsDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? InHandDate { get; set; }
        public string JobName { get; set; }
        public string ShippingConstraintCode { get; set; }

        public string OmsQuoteStatus { get; set; }
        public string QuoteTypeCode { get; set; }
        public int PublishStateId { get; set; }
        public DateTime? QuoteExpirationDate { get; set; }
        public decimal TaxCost { get; set; }
        public decimal ImportDuty { get; set; }

        public decimal ShippingCost { get; set; }
        public decimal? QuoteTotal { get; set; }
        public List<ProductDetailModel> productDetails { get; set; }
        public decimal ShippingHandlingCharges { get; set; }
        public List<TaxSummaryModel> TaxSummaryList { get; set; }
    }
}
