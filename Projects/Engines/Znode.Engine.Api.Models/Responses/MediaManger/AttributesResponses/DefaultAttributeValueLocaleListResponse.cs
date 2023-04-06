using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class DefaultAttributeValueLocaleListResponse : BaseListResponse
    {
        public List<DefaultAttributeValueLocaleModel> DefaultAttributeValueLocales { get; set; }
    }
}
