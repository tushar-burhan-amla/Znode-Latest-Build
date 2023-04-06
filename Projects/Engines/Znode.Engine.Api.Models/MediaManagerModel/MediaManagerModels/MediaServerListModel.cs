using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class MediaServerListModel : BaseListModel
    {
        public List<MediaServerModel> MediaServers { get; set; }

        public MediaServerListModel()
        {
            MediaServers = new List<MediaServerModel>();
        }
    }
}
