using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributeGroupMapperListModel : BaseListModel
    {
        public List<AttributeGroupMapperModel> AttributeGroupMappers { get; set; }

        public AttributeGroupMapperListModel()
        {
            AttributeGroupMappers = new List<AttributeGroupMapperModel>();
        }
    }
}
