using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class ConvertQuoteToOrderModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorQuoteIdRequired)]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorQuoteIdRequired)]
        public int OmsQuoteId { get; set; }
        public int UserId { get; set; }
        public int PortalId { get; set; }
        public SubmitPaymentDetailsModel PaymentDetails { get; set; }
    }
}
