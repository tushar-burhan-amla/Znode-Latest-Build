using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class PIMAttributeFamilyMap
    {
        //This method will map the PIMFamilyGroupAttributeListModel to ZnodePimFamilyGroupMapper Entity.
        public static IEnumerable<ZnodePimFamilyGroupMapper> ToFamilyGroupAttributeListEntity(PIMFamilyGroupAttributeListModel listModel)
        {
            if (listModel?.FamilyAttributeGroups?.Count > 0)
            {
                IEnumerable<ZnodePimFamilyGroupMapper> entityList = listModel.FamilyAttributeGroups.Select(
                    model => new ZnodePimFamilyGroupMapper()
                    {
                        PimAttributeFamilyId = model.AttributeFamilyId,
                        PimAttributeGroupId = model.AttributeGroupId,
                        PimAttributeId = !Equals(model.AttributeId, null) ? model.AttributeId : null,
                        GroupDisplayOrder = !Equals(model.DisplayOrder, null) ? model.DisplayOrder : 999
                    });
                return entityList;
            }
            else
                return null;
        }
    }
}
