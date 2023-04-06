using System;

namespace Znode.Engine.Api.Models
{
    public class ReportAbandonedCartModel
    {
        public string StoreName { get; set; }
        public string CustomerType { get; set; }
        public string Email { get; set; }
        public int Products { get; set; }
        public decimal? Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime CartCreatedAt { get; set; }
        public DateTime CartUpdatedAt { get; set; }
    }
}
