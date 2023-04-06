namespace Znode.Engine.Api.Models
{
    public class PortalProfileModel : BaseModel
    {
        public int PortalProfileID { get; set; }
        public int PortalId { get; set; }
        public int ProfileId { get; set; }
        public bool IsDefaultAnonymousProfile { get; set; }
        public bool IsDefaultRegistedProfile { get; set; }
        public string ProfileName { get; set; }
        public int? ParentProfileId { get; set; }
        public string ProfileNumber { get; set; }
    }
}
