namespace Znode.Libraries.DevExpress.Report
{
    public class OrderInfo : BaseInfo
    {
        public string OrderNumber { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public string BillingCompanyName { get; set; }        
        public string ShippingCity { get; set; }
        public string ShippingStateCode { get; set; }
        public string ShippingCountryCode { get; set; }
        public string BillingPhoneNumber { get; set; }
        public string BillingEmailId { get; set; }
        public decimal TaxCost { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderDate { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public decimal? SalesTax { get; set; }
        public string StoreName { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentTypeName { get; set; }
        public string ShippingTypeName { get; set; }
        public string Symbol { get; set; }
        public string CustomerName { get; set; }
    }
}
