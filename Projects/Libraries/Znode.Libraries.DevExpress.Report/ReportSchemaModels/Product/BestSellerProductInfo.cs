namespace Znode.Libraries.DevExpress.Report
{
    public class BestSellerProductInfo : BaseInfo
    {
        public string StoreName { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmtSold { get; set; }
        public string UnitOfMeasurement { get; set; }
    }
}
