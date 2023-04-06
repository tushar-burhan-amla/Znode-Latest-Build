using System.ComponentModel.DataAnnotations;
using System.Web;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class PaymentDetailsViewModel : BaseViewModel
    {
        public string GatewayName { get; set; }
        public int? PaymentProfileId { get; set; }
        public string Total { get; set; }
        public int PaymentApplicationSettingId { get; set; }
        public int IsCreditCardEnabled { get; internal set; }

        //These properties will use only for 'Purchase Order(PO)' Payemt Type.
        public bool IsPoDocUploadEnable { get; set; }
        public bool IsPoDocRequire { get; set; }
        public bool IsBillingAddressOptional { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.ValidationPurchaseOrderNo)]
        [Display(Name = ZnodeWebStore_Resources.TitlePurchaseOrder, ResourceType = typeof(WebStore_Resources))]
        public string PurchaseOrderNumber { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelPODocument, ResourceType = typeof(WebStore_Resources))]
        [FileMaxSizeValidation(WebStoreConstants.DocumentMaxFileSize, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.FileSizeExceededErrorMessage)]
        [UIHint("FileUploader")]
        public HttpPostedFileBase PODocument { get; set; }
        public string GatewayCode { get; set; }
        public string PaymentCode { get; set; }
        public bool IsOABRequired { get; set; }
        //This property is used to hold Payment Gateway Id 
        public int? PaymentGatewayId { get; set; }
    }
}