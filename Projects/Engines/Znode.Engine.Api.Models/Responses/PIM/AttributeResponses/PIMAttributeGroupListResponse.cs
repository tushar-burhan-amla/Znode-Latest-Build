using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PIMAttributeGroupListResponse : BaseListResponse
    {
        public List<PIMAttributeGroupModel> AttributeGroups { get; set; }
      
        public PIMAttributeGroupMapperListModel AttributeGroupMappers { get; set; }

        public PIMAttributeGroupLocaleListModel AttributeGroupLocales { get; set; }
    }
}
