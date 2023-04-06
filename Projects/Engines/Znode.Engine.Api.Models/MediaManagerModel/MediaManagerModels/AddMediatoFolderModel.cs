namespace Znode.Engine.Api.Models
{
    public class AddMediaToFolderModel : BaseModel
    {
        public int FolderId { get; set; }
        public string MediaIds { get; set; }
    }
}
