namespace Znode.Engine.Api.Models.Responses
{
    public class ContentPageResponse : BaseResponse
    {
        public ContentPageModel ContentPage { get; set; }

        public ContentPageTreeModel Tree { get; set; }

        public ContentPageFolderModel ContentPageFolder { get; set; }
    }
}
