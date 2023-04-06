using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AttributeGroupMapperListResponse : BaseListResponse
    {
        public List<AttributeGroupMapperModel> AttributeGroupMappers { get; set; }
    }
}
