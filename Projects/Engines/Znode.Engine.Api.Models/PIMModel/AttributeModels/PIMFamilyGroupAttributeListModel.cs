using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMFamilyGroupAttributeListModel : BaseListModel
    {
        public List<PIMFamilyGroupAttributeModel> FamilyAttributeGroups { get; set; }

        public PIMFamilyGroupAttributeListModel()
        {
            FamilyAttributeGroups = new List<PIMFamilyGroupAttributeModel>();
        }
    }
}
