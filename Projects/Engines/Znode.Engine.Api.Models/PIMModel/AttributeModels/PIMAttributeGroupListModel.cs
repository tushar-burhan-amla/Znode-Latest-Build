using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeGroupListModel:BaseListModel
    {
        public List<PIMAttributeGroupModel> AttributeGroupList { get; set; }

        public PIMAttributeGroupListModel()
        {
            AttributeGroupList = new List<PIMAttributeGroupModel>();
        }
    }
}
