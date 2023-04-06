using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class AccountQuoteViewModel : BaseViewModel
    {
        public int OmsQuoteId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.HeaderOrderStatus, ResourceType = typeof(Admin_Resources))]
        public int OmsOrderStateId { get; set; }
        public int ShippingId { get; set; }
        public int ShippingAddressId { get; set; }
        public int BillingAddressId { get; set; }
        public int ApproverUserId { get; set; }
        public int AccountId { get; set; }
        public bool IsLastApprover { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelOrderAmount, ResourceType = typeof(Admin_Resources))]
        public string QuoteOrderTotal { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingAmount, ResourceType = typeof(Admin_Resources))]
        public decimal ShippingAmount { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelTaxAmount, ResourceType = typeof(Admin_Resources))]
        public decimal TaxAmount { get; set; }

        public string AdditionalInstruction { get; set; }
        public string UserName { get; set; }
        public string AccountName { get; set; }
        public string OrderStatus { get; set; }
        public string CurrencyCode { get; set; }
        public string UpdatepageType { get; set; }
        public string RoleName { get; set; }
        public string CartItemCount { get; set; }
        public string PermissionCode { get; set; }
        public string OutOfStockMessage { get; set; }
        public string ShippingName { get; set; }
        public string PaymentDisplayName { get; set; }
        public string JobName { get; set; }
        public string BillingAddressHtml { get; set; }
        public string ShippingAddressHtml { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TitleShippingAddress, ResourceType = typeof(Admin_Resources))]
        public string ShippingAddress { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TitleBillingAddress, ResourceType = typeof(Admin_Resources))]
        public string BillingAddress { get; set; }

        public int InventoryOutOfStockCount { get; set; }

        public AddressViewModel BillingAddressModel { get; set; }
        public AddressViewModel ShippingAddressModel { get; set; }
        public CartViewModel ShoppingCart { get; set; }
        public List<SelectListItem> OrderStatusList { get; set; }
        public List<AccountQuoteLineItemViewModel> AccountQuoteLineItemList { get; set; }
        public int LoggedInUserId { get; set; }
        public bool IsLevelApprovedOrRejected { get; set; }
        public bool Permission
        {
            get
            {
                if ((string.Equals(this.RoleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(this.RoleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    && !string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && !string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.ORDERED.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    return true;
                else
                    return false;

            }
        }
        public bool PermissionForAll
        {
            get
            {

                if (string.Equals(this.PermissionCode, ZnodePermissionCodeEnum.DNRA.ToString(), StringComparison.CurrentCultureIgnoreCase)
                   || (string.Equals(this.RoleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(this.RoleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (string.Equals(this.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.APPROVED.ToString(), StringComparison.CurrentCultureIgnoreCase) ||
                    (string.Equals(this.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    && string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.REJECTED.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                    return true;
                else
                    return false;
            }
        }

        public bool PermissionForDraftReject
        {
            get
            {

                if (string.Equals(this.RoleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(this.RoleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || (string.Equals(this.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    || (string.Equals(this.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.REJECTED.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                    return true;
                else
                    return false;
            }
        }

        public int PaymentSettingId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PODocumentName { get; set; }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        public string Comments { get; set; }
        public List<QuoteApprovalViewModel> QuoteApproverComments { get; set; }

        public string CultureCode { get; set; }
        public string OrderNumber { get; set; }
        public int OmsOrderId { get; set; }
        public string OrderType { get; set; }
        public DateTime? InHandDate { get; set; }
        public string ShippingConstraintCode { get; set; }
        public string QuoteNumber { get; set; }
    }
}