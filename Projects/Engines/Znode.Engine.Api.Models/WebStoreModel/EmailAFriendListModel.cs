namespace Znode.Engine.Api.Models
{
    public class EmailAFriendListModel : BaseModel
    {
        public string YourMailId { get; set; }
        public string FriendMailId { get; set; }
        public int LocaleId { get; set; }
        public int CatalogId { get; set; }
        public int PortalId { get; set; }
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
    }
}
