using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributeFamilyListModel : BaseListModel
    {
        public List<AttributeFamilyModel> AttributeFamilies { get; set; }

        public AttributeFamilyListModel()
        {
            AttributeFamilies = new List<AttributeFamilyModel>();
        }
    }
}
