namespace Znode.Engine.Api.Models
{
    public class RecommendationDataGenerateModel : BaseModel
    {
        public int? PortalId { get; set; }
        public bool IsBuildPartial { get; set; }
    }
}
