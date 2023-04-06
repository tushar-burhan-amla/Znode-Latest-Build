using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class OrderItemsRefundViewModel : BaseViewModel
    {
        public int OmsOrderId { get; set; }
        public string OrderNumber { get; set; }
        public string TransactionId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public List<RefundPaymentViewModel> RefundOrderLineitems { get; set; }
        public RefundPaymentViewModel ShippingRefundDetails { get; set; }

        public RefundPaymentViewModel TotalRefundDetails { get; set; }
    }
}