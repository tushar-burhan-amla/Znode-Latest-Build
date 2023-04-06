using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class RecommendationRequestModel : BaseModel
    {
        /// <summary>
        /// ID to identify the customer. Set to null if the customer is not known, or if it is desired to not consider the
        /// customer while recommending products.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// SKU to identify product being viewed. Only applicable when it is desired that products be recommended based on
        /// this specific product as input. This applies, for example, while on the PDP for a product.
        /// </summary>
        public string ProductSkuCurrentlyBeingViewed { get; set; }

        /// <summary>
        /// Skus of recently viewed products.
        /// </summary>
        public List<string> RecentlyViewedProductSkus { get; set; }

        /// <summary>
        /// SKUs of products in cart.
        /// </summary>
        public List<string> ProductSkusInCart { get; set; }

        /// <summary>
        ///  ID of the catalog that is actively being used while fetching recommended products.
        /// </summary>
        public int? CatalogId { get; set; }

        /// <summary>
        /// ID of the locale.
        /// </summary>
        public int LocaleId { get; set; }

        /// <summary>
        /// Id of the profile.
        /// </summary>
        public int? ProfileId { get; set; }

        /// <summary>
        /// Id of the product.
        /// </summary>
        public int PortalId { get; set; }

        /// <summary>
        /// To avoid unwanted API call for recommendations when RecommendationRequestModel is insufficiently populated. 
        /// </summary>
        public bool IsShowRecommendations { get; set; }

        /// <summary>
        /// To hold the widget code.
        /// </summary>
        public string WidgetCode { get; set; }
    }
}