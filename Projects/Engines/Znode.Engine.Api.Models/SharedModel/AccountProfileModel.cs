namespace Znode.Engine.Api.Models
{
    public class AccountProfileModel :  BaseModel
    {
        public int AccountId { get; set; }
        public int AccountProfileId { get; set; }
        public int ProfileId { get; set; }
        public string Name { get; set; }
        public string AccountProfileIds { get; set; }
        public string ProfileIds { get; set; }
    }
}
