using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services.Maps
{
    public static class GlobalAttributeGroupMap
    {
        //This will map znode global attribute group mapper entity list into global attribute group mapper list model.
        public static GlobalAttributeGroupMapperListModel ToListModel(IEnumerable<ZnodeGlobalAttributeGroupMapper> entity)
        {
            if (IsNotNull(entity))
            {
                GlobalAttributeGroupMapperListModel model = new GlobalAttributeGroupMapperListModel() { AttributeGroupMappers = new List<GlobalAttributeGroupMapperModel>() };
                foreach (var item in entity)
                {
                    model.AttributeGroupMappers.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }

        //This will map znode global attribute group mapper entity data into global attributegroup mapper model.
        public static GlobalAttributeGroupMapperModel ToModel(ZnodeGlobalAttributeGroupMapper entity)
        {
            if (IsNotNull(entity))
            {
                return new GlobalAttributeGroupMapperModel
                {
                    GlobalAttributeGroupId = entity.GlobalAttributeGroupId,
                    AttributeDisplayOrder = entity?.ZnodeGlobalAttribute?.DisplayOrder,
                    GlobalAttributeGroupMapperId = entity.GlobalAttributeGroupMapperId,
                    GlobalAttributeId = entity.GlobalAttributeId.GetValueOrDefault(),
                    Attribute = GlobalAttributeMap.ToModel(entity.ZnodeGlobalAttribute)
                };
            }
            else
                return null;
        }

    }
}
