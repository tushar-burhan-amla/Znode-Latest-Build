namespace Znode.Libraries.DevExpress.Report
{
    public class AffiliateOrdersInfo
    {
        public string Name { get; set; }
        public string StoreName { get; set; }
        public int NumberOfOrders { get; set; }
        public decimal OrderValue { get; set; }
        public string CommissionType { get; set; }
        public decimal Commission { get; set; }
        public decimal CommissionOwned { get; set; }
        public int ReferralCommissionTypeID { get; set; }
        public string Symbol { get; set; }
    }
}
