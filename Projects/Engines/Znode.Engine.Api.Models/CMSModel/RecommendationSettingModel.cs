namespace Znode.Engine.Api.Models
{
    public class RecommendationSettingModel : BaseModel
    {
        public int PortalRecommendationSettingId { get; set; }
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public bool IsHomeRecommendation { get; set; }
        public bool IsPDPRecommendation { get; set; }
        public bool IsCartRecommendation { get; set; }
        public string TouchPointName { get; set; }
        public int ERPTaskSchedulerId { get; set; }
    }
}
