namespace Znode.Engine.Api.Models
{
    public class SearchTriggersModel : BaseModel
    {
        public int SearchProfileTriggerId { get; set; }
        public int? ProfileId { get; set; }
        public int SearchProfileId { get; set; }

        public string Keyword { get; set; }
        public string UserProfile { get; set; }
        public string[] ProfileIds { get; set; }

        public bool Status { get; set; }
        public bool IsConfirmation { get; set; }
    }
}
