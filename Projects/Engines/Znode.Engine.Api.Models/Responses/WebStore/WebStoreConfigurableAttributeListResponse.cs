using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreConfigurableAttributeListResponse : BaseListResponse
    {
        public List<PublishAttributeModel> Attributes { get; set; }
    }
}
