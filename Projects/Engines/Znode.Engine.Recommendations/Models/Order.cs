using System.Collections.Generic;

namespace Znode.Engine.Recommendations.Models
{
    public class Order
    {
        public Order()
        {
            Products = new Dictionary<string, decimal?>();
        }

        /// <summary>
        ///     ID of this specific order.
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        ///     Products included in order. String is product ID, int is quantity ordered in this order.
        /// </summary>
        public Dictionary<string, decimal?> Products { get; set; }
    }
}