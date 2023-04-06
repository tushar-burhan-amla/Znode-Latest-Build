using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AttributeEntityGroupListResponse : BaseListResponse
    {
        public List<GlobalAttributeGroupModel> AttributeEntityGroupList { get; set; }
    }
}
