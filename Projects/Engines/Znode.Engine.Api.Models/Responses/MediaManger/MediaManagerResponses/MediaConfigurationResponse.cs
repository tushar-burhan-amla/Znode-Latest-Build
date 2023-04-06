using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class MediaConfigurationResponse : BaseListResponse
    {
        public List<MediaServerModel> MediaServers { get; set; }
        public MediaConfigurationModel MediaConfiguration { get; set; }
    }
}

