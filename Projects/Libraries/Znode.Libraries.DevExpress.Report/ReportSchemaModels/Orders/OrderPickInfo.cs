using System;

namespace Znode.Libraries.DevExpress.Report
{
    public class OrderPickInfo
    {
        public int OmsOrderId { get; set; }
        public string OrderNumber { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public DateTime OrderDate { get; set; }
        public string StoreName { get; set; }
        public string Symbol { get; set; }
    }
}
