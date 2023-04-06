using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class RefundPaymentViewModel : BaseViewModel
    {
        public int OmsPaymentRefundId { get; set; }
        public int? OmsOrderDetailsId { get; set; }
        public int? OmsOrderLineItemsId { get; set; }
        public int? OmsRefundTypeId { get; set; }
        public string RefundType { get; set; }
        public string Notes { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelRefundAmount, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorShippingAmount)]
        public decimal? RefundAmount { get; set; }
        public string ProductName { get; set; }
        public decimal RefundableAmountLeft { get; set; }
        public decimal TotalAmount { get; set; }
    }
}