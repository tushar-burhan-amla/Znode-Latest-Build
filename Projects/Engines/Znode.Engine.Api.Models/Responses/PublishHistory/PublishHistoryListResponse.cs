using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PublishHistoryListResponse : BaseListResponse
    {
        public List<PublishHistoryModel> PublishHistory { get; set; }
    }
}
