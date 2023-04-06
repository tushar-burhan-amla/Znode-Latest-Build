using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class OrderItemsRefundModel : BaseListModel
    {
        public int OmsOrderId { get; set; }
        public string OrderNumber { get; set; }
        public string TransactionId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public List<RefundPaymentModel> RefundOrderLineitems { get; set; }

        public RefundPaymentModel ShippingRefundDetails { get; set; }

        public RefundPaymentModel TotalRefundDetails { get; set; }
        public OrderItemsRefundModel()
        {
            RefundOrderLineitems = new List<RefundPaymentModel>();
        }
    }
}
