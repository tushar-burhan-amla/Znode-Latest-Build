using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
  public class PaymentSettingViewModel : BaseViewModel
  {
    public int PaymentSettingId { get; set; }
    public int PaymentApplicationSettingId { get; set; }

    [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.rfvDisplayOrder)]
    [Range(1, 999999999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidDisplayOrder)]
    [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidDisplayOrder)]
    public int DisplayOrder { get; set; }
    public bool? EnableAmericanExpress { get; set; }
    public bool? EnableDiscover { get; set; }
    public bool? EnableMasterCard { get; set; }
    public bool? EnableRecurringPayments { get; set; }
    public bool? EnableVault { get; set; }
    public bool? EnableVisa { get; set; }
    public bool IsActive { get; set; }
    public bool? IsRmaCompatible { get; set; }
    public bool IsPoDocUploadEnable { get; set; }
    public bool IsPoDocRequire { get; set; }
    public bool IsBillingAddressOptional { get; set; }
    public string PaypalClientId { get; set; }
    public string PaypalClientSecret { get; set; }

    public string Partner { get; set; }
    public PaymentGatewayViewModel PaymentGateway { get; set; }
    public int? PaymentGatewayId { get; set; }
    public string GatewayPassword { get; set; }
    public string GatewayUsername { get; set; }
    public PaymentTypeViewModel PaymentType { get; set; }
    public int PaymentTypeId { get; set; }
    public bool PreAuthorize { get; set; }
    public int? ProfileId { get; set; }
    public bool TestMode { get; set; } = true;
    public string TransactionKey { get; set; }
    public string Vendor { get; set; }
    public string PaymentTypeName { get; set; }
    public string GatewayName { get; set; }

    [Range(0.01, 99999999999.00, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "rnvDisplayOrder")]
    public string AdditionalFee { get; set; }

    public List<BaseDropDownOptions> PaymentTypes { get; set; }
    public List<BaseDropDownOptions> PaymentTypeList { get; set; }
    public List<BaseDropDownOptions> PaymentGateways { get; set; }
    public string PaymentName { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorPaymentCodeRequired)]
    [Display(Name = ZnodeAdmin_Resources.LabelPaymentCode, ResourceType = typeof(Admin_Resources))]
    [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Admin_Resources))]
    [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.PaymentCodeMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
    public string PaymentCode { get; set; }

    [Display(Name = ZnodeAdmin_Resources.LabelPaymentDisplayName, ResourceType = typeof(Admin_Resources))]
    [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterPaymentDisplayName)]
    public string PaymentDisplayName { get; set; }

    public string Notes { get; set; }
    public int PortalId { get; set; }

        public bool IsUsedForOfflinePayment { get; set; }
        public bool IsGuest { get; set; }
        public string PaymentExternalId { get; set; }
        public bool IsCaptureDisable { get; set; }
        public bool IsCallPaymentAPI { get; set; }
        public bool IsApprovalRequired { get; set; }
        public List<SelectListItem> IsApprovalRequiredList { get; set; }
        public string PaymentTypeCode { get; set; }
        public string GatewayCode { get; set; }
        public PaymentSettingViewModel()
        {
            PaymentGateway = new PaymentGatewayViewModel();
            PaymentType = new PaymentTypeViewModel();
        }
        public bool IsOABRequired { get; set; }
        public List<SelectListItem> IsOABRequiredList { get; set; }
        public string PublishState { get; set; }
        public List<SelectListItem> PublishStateList { get; set; }
        //Get the value of IsOABSupported for a payment method.
        public virtual bool IsOABSupported()
        {
          return !this.IsCallPaymentAPI;
        }
  }
}