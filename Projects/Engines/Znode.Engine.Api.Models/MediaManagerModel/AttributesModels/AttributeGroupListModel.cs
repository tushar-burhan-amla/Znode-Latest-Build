using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributeGroupListModel : BaseListModel
    {
        public List<AttributeGroupModel> AttributeGroups { get; set; }

        public AttributeGroupListModel()
        {
            AttributeGroups = new List<AttributeGroupModel>();
        }
    }
}
