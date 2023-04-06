using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CMSSearchIndexServerStatusListResponse : BaseListResponse
    {
        public List<CMSSearchIndexServerStatusModel> CMSSearchIndexServerStatusList { get; set; }
    }
}