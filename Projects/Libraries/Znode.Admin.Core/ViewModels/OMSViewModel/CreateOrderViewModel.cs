using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CreateOrderViewModel : BaseViewModel
    {
        [Display(Name = ZnodeAdmin_Resources.LabelStore, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TitleCatalog, ResourceType = typeof(Admin_Resources))]
        public int CatalogId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAccounts, ResourceType = typeof(Admin_Resources))]
        public int AccountId { get; set; }

        public List<SelectListItem> PortalList { get; set; }
        public List<SelectListItem> CatalogList { get; set; }
        public List<SelectListItem> AccountList { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCustomerName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.CustomerNameRequiredError)]
        public string CustomerName { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }

        public UserAddressDataViewModel UserAddressDataViewModel { get; set; }

        public CartViewModel CartViewModel { get; set; }

        public ShippingListViewModel ShippingListViewModel { get; set; }

        public PaymentSettingViewModel PaymentSettingViewModel { get; set; }

        public ReviewOrderViewModel ReviewOrderViewModel { get; set; }
        public int? ShippingId { get; set; }
        public int PortalCatalogId { get; set; }
        public int PaymentTypeId { get; set; }
        public int UserBillingAddressId { get; set; }
        public int UserShippingAddressId { get; set; }
        public int OrderId { get; set; }
        public string StoreName { get; set; }
        [AllowHtml]
        public string ReceiptHtml { get; set; }
        public bool EnableAddressValidation { get; set; }
        public bool RequireValidatedAddress { get; set; }
        public bool IsQuote { get; set; }
        public string AdditionalInstructions { get; set; }
        public string AdditionalNotes { get; set; }
        public List<OrderNotesViewModel> OrderNotes { get; set; }
        public bool IsEmailSend { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string OrderNumber { get; set; }
        public string UpdatePageType { get; set; }
        public int SendUserId { get; set; }
        public string CreditCardNumber { get; set; }
        public bool IsFromUserCart { get; set; }
        public bool IsGuestUser { get; set; }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelInHandsDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? InHandDate { get; set; }
        public string JobName { get; set; }
        public string ShippingConstraintCode { get; set; }
    }
}