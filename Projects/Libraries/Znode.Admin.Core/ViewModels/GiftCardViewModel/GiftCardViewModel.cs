using System;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class GiftCardViewModel : BaseViewModel
    {
        public int GiftCardId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TextStoreName, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }

        [RegularExpression(AdminConstants.NumberExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.MessageNumericValueAllowed)]
        [Display(Name = ZnodeAdmin_Resources.LabelCustomerId, ResourceType = typeof(Admin_Resources))]
        [Range(1, 99999999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.rnvDisplayOrder)]
        public int? UserId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelVoucherName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.GiftCardNameMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterVoucherName)]
        public string Name { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelVoucherNumber, ResourceType = typeof(Admin_Resources))]
        public string CardNumber { get; set; }
        public string DisplayAmount { get; set; }
        public string CurrencyCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelVoucherAmount, ResourceType = typeof(Admin_Resources))]
        [Range(0.01, 999999.00, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRangeBetween)]
        public string Amount { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAmountOwed, ResourceType = typeof(Admin_Resources))]
        public decimal OwedAmount { get; set; }
        public decimal LeftAmount { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelRemainingAmount, ResourceType = typeof(Admin_Resources))]
        [Range(0.01, 999999.00, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRangeBetween)]
        public string RemainingAmount { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelVoucherAmount, ResourceType = typeof(Admin_Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterVoucherAmount)]
        [Range(0.01, 999999.00, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRangeBetween)]
        public decimal GiftCardAmount { get; set; }

        public int? AccountId { get; set; }
        public bool RestrictToCustomerAccount { get; set; }
        public bool IsReferralCommission { get; set; } = false;

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelStartDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? StartDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelExpirationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = ZnodeAdmin_Resources.IsActive, ResourceType = typeof(Admin_Resources))]
        public bool IsActive { get; set; }
        public bool SendMail { get; set; }

        public RMARequestViewModel RmaRequestModel { get; set; }

        public string StoreName { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LblCustomerName, ResourceType = typeof(Admin_Resources))]
        public string CustomerName { get; set; }
        public string CultureCode { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
    }
}