namespace Znode.Engine.Recommendations.Models
{
    //Model to hold SKU and Quantity of product.
    public class RecommendationLineItemModel
    {
        public string SKU { get; set; }
        public decimal? Quantity { get; set; }
    }
}
