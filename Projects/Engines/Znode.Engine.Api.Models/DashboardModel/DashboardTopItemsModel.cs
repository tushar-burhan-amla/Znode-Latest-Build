namespace Znode.Engine.Api.Models
{
    public class DashboardTopItemsModel : DashboardModel
    {
        public string Products { get; set; }
        public string Categories { get; set; }
        public string Brands { get; set; }
        public string Searches { get; set; }
        public string TotalSales { get; set; }
        public int Times { get; set; }
        public int TotalOrders { get; set; }
        public int TotalNewCustomer { get; set; }
        public int ProductCount { get; set; }
        public string Type { get; set; }
        public string SKU { get; set; }
    }
}
