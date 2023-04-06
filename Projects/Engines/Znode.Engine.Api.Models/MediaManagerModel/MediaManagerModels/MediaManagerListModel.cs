using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class MediaManagerListModel : BaseListModel
    {
        public List<MediaManagerModel> MediaList { get; set; }
        public MediaManagerListModel()
        {
            MediaList = new List<MediaManagerModel>();
        }
    }
}
