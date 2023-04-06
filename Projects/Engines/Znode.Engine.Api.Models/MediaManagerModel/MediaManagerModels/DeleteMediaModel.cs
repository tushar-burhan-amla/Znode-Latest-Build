namespace Znode.Engine.Api.Models
{
    public class DeleteMediaModel : BaseModel
    {
        public int FolderId { get; set; }
        public string MediaIds { get; set; }
    }
}
