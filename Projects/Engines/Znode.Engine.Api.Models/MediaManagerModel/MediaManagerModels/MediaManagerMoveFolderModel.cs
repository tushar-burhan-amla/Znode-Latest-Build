namespace Znode.Engine.Api.Models
{
    public class MediaManagerMoveFolderModel : BaseModel
    {
        public int ParentId { get; set; }
        public int FolderId { get; set; }
        public int AccountId { get; set; }
    }
}
