using System;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Recommendations.Models
{
    public class RecommendationProcessingLogModel : BaseModel
    {
        public int RecommendationProcessingLogsId { get; set; }

        //Will be set as null if the model generation is started by considering all the orders irrespective of portal.
        public int? PortalId { get; set; }

        //Will be saved as Complete or Processing.
        public string Status { get; set; }

        //Id of last processed order.
        public int LastProcessedOrderId { get; set; }

        //Date of last processed order.
        public DateTime LastProcessedOrderDate { get; set; }
    }
}
