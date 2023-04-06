namespace Znode.Engine.Api.Models.Responses
{
    public class QuoteListResponse : BaseListResponse
    {
        public QuoteListModel QuoteList { get; set; }
        public string PortalName { get; set; }
    }
}
