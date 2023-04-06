using System;

namespace Znode.Engine.Admin.ViewModels
{
    public class DashboardItemsViewModel : BaseViewModel
    {
        public string ItemName { get; set; }
        public string CustomerName { get; set; }
        public DateTime? Date { get; set; }
        public decimal Total { get; set; }
        public string ItemId { get; set; }
        public string Type { get; set; }
        public int TotalOrders { get; set; }
        public int TotalQuotes { get; set; }
        public int TotalReturns { get; set; }
        public string TotalSales { get; set; }
        public string Symbol { get; set; }
        public string ItemCode { get; set; }
    }
}
