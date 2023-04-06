namespace Znode.Engine.Api.Models
{
    public class PublishPortalLogModel : BaseModel
    {
        public int PublishPortalLogId { get; set; }
        public bool? IsPortalPublished { get; set; }
        public string PublishStatus { get; set; }
        public string UserName { get; set; }
        public int? PublishCategoryCount { get; set; }
    }
}
