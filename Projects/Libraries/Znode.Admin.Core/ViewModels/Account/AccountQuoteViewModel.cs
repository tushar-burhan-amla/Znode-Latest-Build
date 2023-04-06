using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class AccountQuoteViewModel : BaseViewModel
    {
        public int OmsQuoteId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.HeaderQuoteStatus, ResourceType = typeof(Admin_Resources))]
        public int OmsOrderStateId { get; set; }
        public int ShippingId { get; set; }
        public int ShippingAddressId { get; set; }
        public int BillingAddressId { get; set; }
        public int ApproverUserId { get; set; }
        public int AccountId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.HeaderQuoteAmount, ResourceType = typeof(Admin_Resources))]
        public string QuoteOrderTotal { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingAmount, ResourceType = typeof(Admin_Resources))]
        public decimal ShippingAmount { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelDiscountAmount, ResourceType = typeof(Admin_Resources))]
        public decimal DiscountAmount { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelTaxAmount, ResourceType = typeof(Admin_Resources))]
        public decimal TaxAmount { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelImportDuty, ResourceType = typeof(Admin_Resources))]
        public decimal ImportDuty { get; set; }       
        public string AdditionalInstruction { get; set; }
        public string UserName { get; set; }
        public string AccountName { get; set; }
        public string OrderStatus { get; set; }
        public string CurrencyCode { get; set; }
        public string UpdatePageType { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TitleShippingAddress, ResourceType = typeof(Admin_Resources))]
        public string ShippingAddress { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TitleBillingAddress, ResourceType = typeof(Admin_Resources))]
        public string BillingAddress { get; set; }
        public string AdditionalNotes { get; set; }
        public List<OrderNotesViewModel> OrderNotes { get; set; }
        public CartViewModel ShoppingCart { get; set; }
        public List<SelectListItem> OrderStatusList { get; set; }
        public List<AccountQuoteLineItemViewModel> AccountQuoteLineItemList { get; set; }
        public string StoreName { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public string BillingAccountNumber { get; set; }
        public bool IsConvertedToOrder { get; set; }

        public string OrderNumber { get; set; }
        public string CultureCode { get; set; }
        public string CountryCode { get; set; }        
        public string PurchaseOrderNumber { get; set; }
        public string PODocumentName { get; set; }
        public string PublishState { get; set; }
        public string AccountCode { get; set; }
        public string ShippingTypeClassName { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelJobName, ResourceType = typeof(Admin_Resources))]
        public string JobName { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelInHandsDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? InHandDate { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelShippingConstraintsCode, ResourceType = typeof(Admin_Resources))]
        public string ShippingConstraintCode { get; set; }
        public string ShippingTypeDescription { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelShippingMethod, ResourceType = typeof(Admin_Resources))]
        public string ShippingMethod { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelAccountNumber, ResourceType = typeof(Admin_Resources))]
        public string AccountNumber { get; set; }

        public string PhoneNumber { get; set; }
        public IList<ShippingConstraintsViewModel> ShippingConstraints { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PaymentDisplayName { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? ShippingDiscount { get; set; }
        public decimal? ShippingHandlingCharges { get; set; }
        public AddressViewModel BillingAddressModel { get; set; }
        public AddressViewModel ShippingAddressModel { get; set; }
        public string QuoteNumber { get; set; }
        public int OmsOrderId { get; set; }
    }
}