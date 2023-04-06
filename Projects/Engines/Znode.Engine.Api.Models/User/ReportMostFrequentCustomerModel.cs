namespace Znode.Engine.Api.Models
{
    public class ReportMostFrequentCustomerModel
    {
        public string StoreName { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int OrderCount { get; set; }
        public decimal Total { get; set; }
        public string CustomerType { get; set; }
        public string Symbol { get; set; }
    }
}
