using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class FamilyGroupAttributeMap
    {
        //This method will map the list of ZnodeFamilyGroupMapper Entity to FamilyGroupAttribute List Model.
        public static FamilyGroupAttributeListModel ToListModel(IList<ZnodeMediaFamilyGroupMapper> entityList)
        {
            if (!Equals(entityList, null))
            {
                var model = new FamilyGroupAttributeListModel();
                foreach (var entity in entityList)
                {
                    model.FamilyGroupAttributes.Add(ToModel(entity));
                }
                return model;
            }
            else
                return new FamilyGroupAttributeListModel();
        }

        //This method will map the ZnodeFamilyGroupMapper Entity to FamilyGroupAttribute Model.
        public static FamilyGroupAttributeModel ToModel(ZnodeMediaFamilyGroupMapper entity)
        {
            if (!Equals(entity, null))
            {
                return new FamilyGroupAttributeModel
                {
                    FamilyGroupAttributeId = entity.MediaFamilyGroupMapperId,
                    AttributeFamilyId = entity.MediaAttributeFamilyId.GetValueOrDefault(),
                    AttributeGroupId = entity.MediaAttributeGroupId.GetValueOrDefault(),
                    AttributeGroupName = entity.ZnodeMediaAttributeGroup.GroupCode,
                };
            }
            else
                return new FamilyGroupAttributeModel();
        }
    }
}
