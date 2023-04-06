using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class MediaManagerTreeModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
        public List<MediaManagerTreeModel> Children { get; set; }
        public int ParentId { get; set; }
    }
}
