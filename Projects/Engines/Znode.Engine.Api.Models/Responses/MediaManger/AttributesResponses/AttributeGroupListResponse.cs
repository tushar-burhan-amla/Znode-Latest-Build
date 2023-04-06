using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AttributeGroupListResponse : BaseListResponse
    {
        public List<AttributeGroupModel> AttributeGroups { get; set; }

        public AttributeGroupMapperListModel AttributeGroupMappers { get; set; }

        public AttributeGroupLocaleListModel AttributeGroupLocales { get; set; }
    }
}
