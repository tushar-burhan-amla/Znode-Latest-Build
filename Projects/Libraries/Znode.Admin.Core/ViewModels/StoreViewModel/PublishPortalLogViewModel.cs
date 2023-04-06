namespace Znode.Engine.Admin.ViewModels
{
    public class PublishPortalLogViewModel : BaseViewModel
    {
        public int PublishPortalLogId { get; set; }
        public int? PublishPortalId { get; set; }
        public bool? IsPortalPublished { get; set; }
        public string PublishStatus { get; set; }
        public string UserName { get; set; }
        public int? PublishCategoryCount { get; set; }
    }
}
