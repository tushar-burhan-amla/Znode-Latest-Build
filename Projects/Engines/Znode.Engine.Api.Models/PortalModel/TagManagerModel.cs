namespace Znode.Engine.Api.Models
{
    public class TagManagerModel : BaseModel
    {
        public int GoogleTagManagerId { get; set; }
        public int PortalId { get; set; }

        public bool IsActive { get; set; }

        public string PortalName { get; set; }
        public string ContainerId { get; set; }
        public string AnalyticsIdForAddToCart { get; set; }
        public string AnalyticsIdForRemoveFromCart { get; set; }
        public string AnalyticsUId { get; set; }
        public bool AnalyticsIsActive { get; set; }
        public bool EnableEnhancedEcommerce { get; set; }
    }
}
