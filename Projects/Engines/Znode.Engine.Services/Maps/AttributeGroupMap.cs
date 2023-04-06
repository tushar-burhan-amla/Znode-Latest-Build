using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class AttributeGroupMap
    {
        //Convert ZnodeMediaAttributeGroupMapper model into AttributeGroupMapperModel.
        public static AttributeGroupMapperModel ToModel(ZnodeMediaAttributeGroupMapper entity)
        {
            if (!Equals(entity, null))
            {
                return new AttributeGroupMapperModel
                {
                    MediaAttributeGroupId = entity.MediaAttributeGroupId,
                    MediaAttributeGroupMapperId = entity.MediaAttributeGroupMapperId,
                    MediaAttributeId = entity.MediaAttributeId.GetValueOrDefault(),
                    IsSystemDefined = entity.IsSystemDefined,
                    AttributeDisplayOrder= entity.ZnodeMediaAttribute.DisplayOrder,
                    Attribute = entity.ZnodeMediaAttribute?.ToModel<AttributesDataModel>()
                };
            }
            else
                return null;
        }

        //Convert IEnumerable<ZnodeMediaAttributeGroupMapper> list into AttributeGroupMapperListModel model.
        public static AttributeGroupMapperListModel ToListModel(IEnumerable<ZnodeMediaAttributeGroupMapper> entity)
        {
            if (!Equals(entity, null))
            {
                AttributeGroupMapperListModel model = new AttributeGroupMapperListModel() { AttributeGroupMappers = new List<AttributeGroupMapperModel>() };
                foreach (var item in entity)
                {
                    model.AttributeGroupMappers.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }

        //Map ZnodeMediaAttribute Entity Model to AttributesListDataModel 
        public static AttributesListDataModel AddGroupNameToListModel(IList<ZnodeMediaAttribute> entity)
        {
            if (!Equals(entity, null))
            {
                AttributesListDataModel model = new AttributesListDataModel();
                model.Attributes = (from _item in entity
                                    select new AttributesDataModel
                                    {
                                        MediaAttributeId = _item.MediaAttributeId,
                                        AttributeTypeId = _item.AttributeTypeId,
                                        AttributeCode = _item.AttributeCode,
                                        AttributeName =_item.ZnodeMediaAttributeLocales.Count > 0 ? _item.ZnodeMediaAttributeLocales?.Where(x => x.LocaleId == Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale)).Select(x => x.AttributeName).FirstOrDefault().ToString() : _item.AttributeCode,
                                        IsRequired = _item.IsRequired,
                                        IsLocalizable = _item.IsLocalizable,
                                        IsFilterable = _item.IsFilterable,
                                        IsSystemDefined = _item.IsSystemDefined,
                                        HelpDescription = _item.HelpDescription,
                                        DisplayOrder = _item.DisplayOrder
                                    }).ToList();
                return model;
            }
            else
                return null;
        }
    }
}
