namespace Znode.Engine.Api.Models
{
    public class RobotsTxtModel : BaseModel
    {
        public int RobotsTxtId { get; set; }
        public int PortalId { get; set; }
        public string StoreName { get; set; }
        public string RobotsTxtContent { get; set; }
        public string DefaultRobotTag { get; set; }
    }
}
