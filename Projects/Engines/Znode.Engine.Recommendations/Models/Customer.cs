using System.Collections.Generic;

namespace Znode.Engine.Recommendations.Models
{
    public class Customer
    {
        public Customer()
        {
            Orders = new List<Order>();
        }

        /// <summary>
        ///     ID of the customer.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        ///     Orders purchased by customer.
        /// </summary>
        public List<Order> Orders { get; set; }
    }
}