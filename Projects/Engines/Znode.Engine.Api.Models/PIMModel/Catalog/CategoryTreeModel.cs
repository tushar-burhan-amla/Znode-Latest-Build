namespace Znode.Engine.Api.Models
{
    public class CategoryTreeModel : BaseModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public bool children { get; set; } = true;

    }
}
