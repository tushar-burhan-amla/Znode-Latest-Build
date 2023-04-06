using System.Collections.Generic;

namespace Znode.Engine.Recommendations.Models
{
    public class Recommendation
    {
        internal Recommendation()
        {
            ProductSkus = new List<string>();
        }

        /// <summary>
        ///     The keys of the products that are recommended.
        /// </summary>
        public List<string> ProductSkus { get; set; }

        /// <summary>
        ///     The context that was considered while determining which products to recommend.
        /// </summary>
        public RecommendationContext RecommendationContext { get; set; }
    }
}