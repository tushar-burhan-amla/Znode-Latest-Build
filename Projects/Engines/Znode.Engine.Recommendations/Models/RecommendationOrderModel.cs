using System;
using System.Collections.Generic;

namespace Znode.Engine.Recommendations.Models
{
    public class RecommendationOrderModel
    {
        /// <summary>
        /// To hold last processed order Date
        /// </summary>
        public DateTime LastProcessedOrderDate { get; set; }

        /// <summary>
        /// To hold last processed order Id
        /// </summary>
        public int LastProcessedOrderId { get; set; }

        /// <summary>
        /// To hold line items sku list of orders
        /// </summary>
        public List<List<RecommendationLineItemModel>> LineItemsSkuList { get; set; }
    }
}
