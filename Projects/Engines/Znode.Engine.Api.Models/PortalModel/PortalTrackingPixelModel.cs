namespace Znode.Engine.Api.Models
{
    public class PortalTrackingPixelModel : BaseModel
    {
        public int PortalId { get; set; }
        public int PortalPixelTrackingId { get; set; }
        public string StoreName { get; set; }
        public string TrackingPixelScriptCode { get; set; }
    }
}
