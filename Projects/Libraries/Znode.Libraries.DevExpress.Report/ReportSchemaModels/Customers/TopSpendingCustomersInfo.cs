namespace Znode.Libraries.DevExpress.Report
{
    public class TopSpendingCustomersInfo : BaseInfo
    {
        public string StoreName { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerType { get; set; }
        public string Symbol { get; set; }
        public decimal AverageTotalAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TotalTaxCost { get; set; }
        public decimal TotalShippingCost { get; set; }
        public decimal TotalDiscount { get; set; }
    }
}
