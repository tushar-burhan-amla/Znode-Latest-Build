namespace Znode.Libraries.DevExpress.Report
{
    public class TopEarningProductInfo : BaseInfo
    {
        public string StoreName { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public int QtySold { get; set; }
        public decimal SoldPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
