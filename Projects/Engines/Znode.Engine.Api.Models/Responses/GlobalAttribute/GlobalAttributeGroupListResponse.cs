using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class GlobalAttributeGroupListResponse : BaseListResponse
    {
        public List<GlobalAttributeGroupModel> AttributeGroups { get; set; }

        public GlobalAttributeGroupMapperListModel AttributeGroupMappers { get; set; }

        public GlobalAttributeGroupLocaleListModel AttributeGroupLocales { get; set; }
    }
}