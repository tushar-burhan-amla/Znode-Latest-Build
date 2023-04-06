namespace Znode.Engine.Api.Models
{
    public class CMSPageSearchRequestModel : BaseModel
    {
        public int PortalId { get; set; }
        public int LocaleId { get; set; }
        public int ProfileId { get; set; }
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
    }
}
