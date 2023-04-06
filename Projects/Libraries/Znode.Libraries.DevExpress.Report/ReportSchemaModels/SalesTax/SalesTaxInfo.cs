using System;

namespace Znode.Libraries.DevExpress.Report
{
    public class SalesTaxInfo : BaseInfo
    {
        public string StoreName { get; set; }
        public DateTime OrderDate { get; set; }
        public string State { get; set; }
        public decimal TotalSales { get; set; }
        public decimal Tax { get; set; }
        public string Title { get; set; }
        public string Custom1 { get; set; }
        public string Symbol { get; set; }
    }
}
