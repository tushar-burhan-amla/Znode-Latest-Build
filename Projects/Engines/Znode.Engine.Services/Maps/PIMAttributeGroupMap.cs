using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class PIMAttributeGroupMap
    {
        public static PIMAttributeGroupMapperModel ToModel(ZnodePimAttributeGroupMapper entity)
        {
            if (!Equals(entity, null))
            {
                return new PIMAttributeGroupMapperModel
                {
                    PimAttributeGroupId = entity.PimAttributeGroupId,
                    AttributeDisplayOrder = entity?.ZnodePimAttribute?.DisplayOrder,
                    PimAttributeGroupMapperId = entity.PimAttributeGroupMapperId,
                    PimAttributeId = entity.PimAttributeId.GetValueOrDefault(),
                    IsSystemDefined = entity.IsSystemDefined,
                    Attribute = PIMAttributesMap.ToModel(entity.ZnodePimAttribute)
                };
            }
            else
                return null;
        }

        public static PIMAttributeGroupMapperListModel ToListModel(IEnumerable<ZnodePimAttributeGroupMapper> entity)
        {
            if (!Equals(entity, null))
            {
                PIMAttributeGroupMapperListModel model = new PIMAttributeGroupMapperListModel() { AttributeGroupMappers = new List<PIMAttributeGroupMapperModel>() };
                foreach (var item in entity)
                {
                    model.AttributeGroupMappers.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }

        //Map ZnodePimAttributeGroup Entity Model to PIMAttributeGroupListModel 
        public static PIMAttributeGroupListModel AddGroupNameToListModel(IList<ZnodePimAttributeGroup> entity)
        {
            if (!Equals(entity, null))
            {
                PIMAttributeGroupListModel model = new PIMAttributeGroupListModel();
                model.AttributeGroupList = (from _item in entity
                                            select new PIMAttributeGroupModel
                                            {
                                                PimAttributeGroupId=_item.PimAttributeGroupId,
                                                GroupCode=_item.GroupCode,
                                                IsSystemDefined=_item.IsSystemDefined,
                                                AttributeGroupName=_item.ZnodePimAttributeGroupLocales.Count > 0 ? _item.ZnodePimAttributeGroupLocales?.Where(x => x.LocaleId == Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale)).Select(x => x.AttributeGroupName).FirstOrDefault().ToString() : string.Empty,
                                                DisplayOrder = _item.DisplayOrder,
                                                IsCategory=_item.IsCategory,
                                                IsNonEditable=_item.IsNonEditable,
                                            }).ToList();
                return model;
            }
            else
                return null;
        }
    }
}
