using Znode.Engine.Api.Models;

namespace Znode.Engine.Recommendations.Models
{
    public class RecommendedProductModel : BaseModel
    {
        public long RecommendedProductsId { get; set; }
        public long RecommendationBaseProductsId { get; set; }
        public string SKU { get; set; }
        public decimal? Quantity { get; set; }
        public int Occurrence { get; set; }
    }
}
