using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Znode.Engine.WebStore.Agents;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class CheckoutViewModel : BaseViewModel
    {
        public int UserId { get; set; }
        public int QuoteId { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string RoleName { get; set; }
        public string OrderStatus { get; set; }
        public string PermissionCode { get; set; }
        public int ShippingId { get; set; }
        public string CreditCardNumber { get; set; }
        public bool IsRequireApprovalRouting { get; set; }
        public string ApprovalType { get; set; }
        public bool ApproverCount { get; set; }
        public bool IsLastApprover { get; set; }
        public bool IsLevelApprovedOrRejected { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelInHandsDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? InHandDate { get; set; }
        public IList<ShippingConstraintsViewModel> ShippingConstraints { get; set; }
        [Display(Name = ZnodeWebStore_Resources.LabelShippingConstraintsCode, ResourceType = typeof(WebStore_Resources))]
        public string ShippingConstraintCode { get; set; }
        public bool Permission
        {
            get
            {
                if ((string.Equals(this.RoleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(this.RoleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(this.PermissionCode, ZnodePermissionCodeEnum.DNRA.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || (string.Equals(this.PermissionCode, ZnodePermissionCodeEnum.ARA.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.APPROVED.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    || (string.Equals(this.PermissionCode, ZnodePermissionCodeEnum.SRA.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && (this.SubTotal <= this.BudgetAmount) && !string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.REJECTED.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.APPROVED.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    || string.Equals(this.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(this.RoleName, ZnodeRoleEnum.Admin.ToString(), StringComparison.CurrentCultureIgnoreCase) || Equals(this.RoleName, null)))
                    return true;
                else
                    return false;
            }
        }

        public bool SumbitApproval
        {
            get
            {
                if (string.Equals(this.PermissionCode, ZnodePermissionCodeEnum.ARA.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || (string.Equals(this.PermissionCode, ZnodePermissionCodeEnum.SRA.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && (this.SubTotal > this.BudgetAmount || string.Equals(this.OrderStatus, ZnodeOrderStatusEnum.REJECTED.ToString(), StringComparison.CurrentCultureIgnoreCase))))
                    return true;
                else
                    return false;
            }
        }

        public bool SubmitDraft
        {
            get
            {
                if (string.Equals(this.RoleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(this.RoleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(this.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool EnableApprovalRouting
        {
            get; set;
        }

        public bool ShowPlaceOrderButton
        {
            get; set;

        }
        public decimal OrderLimit { get; set; }

        public bool IsQuoteRequest { get; set; }

        public bool IsPendingOrderRequest { get; set; }
    }
}