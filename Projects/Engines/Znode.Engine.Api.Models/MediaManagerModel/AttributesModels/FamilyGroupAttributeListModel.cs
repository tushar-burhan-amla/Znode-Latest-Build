using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class FamilyGroupAttributeListModel : BaseListModel
    {
        public List<FamilyGroupAttributeModel> FamilyGroupAttributes { get; set; }

        public FamilyGroupAttributeListModel()
        {
            FamilyGroupAttributes = new List<FamilyGroupAttributeModel>();
        }
    }
}
