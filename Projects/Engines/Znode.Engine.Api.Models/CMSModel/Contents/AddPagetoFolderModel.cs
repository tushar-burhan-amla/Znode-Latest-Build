namespace Znode.Engine.Api.Models
{
    public class AddPageToFolderModel : BaseModel
    {
        public int FolderId { get; set; }
        public string PageIds { get; set; }
    }
}
