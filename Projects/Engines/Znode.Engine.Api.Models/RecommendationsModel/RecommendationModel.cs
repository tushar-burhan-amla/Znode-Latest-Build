using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class RecommendationModel : BaseModel
    {
        public RecommendationModel()
        {
            RecommendedProducts = new List<PublishProductModel>();
        }

        /// <summary>
        /// To hold the recommended products detail.
        /// </summary>
        public List<PublishProductModel> RecommendedProducts { get; set; }
    }
}
