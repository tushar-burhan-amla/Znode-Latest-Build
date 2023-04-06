using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeGroupListModel : BaseListModel
    {
        public List<GlobalAttributeGroupModel> AttributeGroupList { get; set; }

        public GlobalAttributeGroupListModel()
        {
            AttributeGroupList = new List<GlobalAttributeGroupModel>();
        }
    }
}
