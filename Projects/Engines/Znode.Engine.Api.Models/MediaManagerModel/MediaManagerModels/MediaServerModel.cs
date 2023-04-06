namespace Znode.Engine.Api.Models
{
    public class MediaServerModel : BaseModel
    {
        public string ServerName { get; set; }
        public string PartialViewName { get; set; }
        public int MediaServerMasterId { get; set; }
        public string ClassName { get; set; }
        public bool? IsOtherServer { get; set; }
        public string URL { get; set; }
    }
}
