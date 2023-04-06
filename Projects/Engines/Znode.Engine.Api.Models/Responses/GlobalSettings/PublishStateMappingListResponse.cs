using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PublishStateMappingListResponse : BaseListResponse
    {
        public List<PublishStateMappingModel> PublishStateMappings { get; set; }
    }
}
