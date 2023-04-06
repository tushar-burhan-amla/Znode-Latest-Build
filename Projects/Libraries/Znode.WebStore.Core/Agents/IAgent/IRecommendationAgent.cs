using System.Collections.Generic;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface IRecommendationAgent
    {
        /// <summary>
        /// Get recommended products.
        /// </summary>
        /// <param name="widgetCode">Widget Code</param>
        /// <param name="productSku">Product SKU</param>
        /// <returns>List of recommended products.</returns>
        List<ProductViewModel> GetRecommendedProducts(string widgetCode, string productSku);        
    }
}
