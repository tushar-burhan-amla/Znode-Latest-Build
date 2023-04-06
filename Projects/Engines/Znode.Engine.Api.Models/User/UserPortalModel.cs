namespace Znode.Engine.Api.Models
{
    public class UserPortalModel : BaseModel
    {
        public int UserId { get; set; }
        public int UserPortalId { get; set; }

        public string AspNetUserId { get; set; }
        public string[] PortalIds { get; set; }       
    }
}
