using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Recommendations.Models
{
    public class RecommendationBaseProductModel : BaseModel
    {
        public long RecommendationBaseProductsId { get; set; }

        public string SKU { get; set; }

        //Will be set as null if the model generation is started by considering all the orders irrespective of portal.
        public int? PortalId { get; set; }

        public int RecommendationProcessingLogsId { get; set; }

        //List of recommended products against the base product
        public List<RecommendedProductModel> RecommendedProducts { get; set; }
    }
}
