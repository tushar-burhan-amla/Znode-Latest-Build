using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class AttributeFamilyMap
    {
        public static AttributeFamilyModel ToModel(ZnodeMediaAttributeFamily entity)
        {
            if (!Equals(entity, null))
            {
                return new AttributeFamilyModel
                {
                    MediaAttributeFamilyId = entity.MediaAttributeFamilyId,
                    FamilyCode = entity.FamilyCode,
                    IsDefaultFamily = entity.IsDefaultFamily,
                    CreatedBy = entity.CreatedBy,
                    ModifiedBy = entity.ModifiedBy,
                    IsSystemDefined = entity.IsSystemDefined,
                    CreatedDate = entity.CreatedDate,
                    ModifiedDate = entity.ModifiedDate
                };
            }
            else
                return null;
        }

        public static IEnumerable<ZnodeMediaFamilyGroupMapper> ToFamilyGroupAttributeListEntity(FamilyGroupAttributeListModel listModel)
        {
            if (listModel?.FamilyGroupAttributes?.Count > 0)
            {
                IEnumerable<ZnodeMediaFamilyGroupMapper> entityList = listModel.FamilyGroupAttributes.Select(
                    model => new ZnodeMediaFamilyGroupMapper()
                    {
                        MediaAttributeFamilyId = model.AttributeFamilyId,
                        MediaAttributeGroupId = model.AttributeGroupId,
                        MediaAttributeId = !Equals(model.AttributeId, null) ? model.AttributeId : null,
                        GroupDisplayOrder = model.GroupDisplayOrder
                    });
                return entityList;
            }
            else
                return null;
        }

        //Convert IList<ZnodeMediaAttributeFamily> in to AttributeFamilyListModel model.
        public static AttributeFamilyListModel ToAttributeFamilyListModel(IList<ZnodeMediaAttributeFamily> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new AttributeFamilyListModel();
                foreach (ZnodeMediaAttributeFamily item in entity)
                {
                    model.AttributeFamilies.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }

        //Convert ZnodeMediaFamilyLocale entity model to FamilyLocaleListModel model.
        public static FamilyLocaleListModel ToLocaleListModel(IList<ZnodeMediaFamilyLocale> entity)
        {
            if (!Equals(entity, null))
            {
                FamilyLocaleListModel model = new FamilyLocaleListModel();
                model.FamilyLocales = (from _item in entity
                                       select new FamilyLocaleModel
                                       {
                                           LocaleId = _item.LocaleId,
                                           AttributeFamilyId = _item.MediaAttributeFamilyId,
                                           FamilyLocaleId = _item.MediaFamilyLocaleId,
                                           AttributeFamilyName = _item.AttributeFamilyName
                                       }).ToList();
                return model;
            }
            return null;
        }

        //Convert IList<ZnodeMediaAttributeGroupMapper> in to AttributeGroupMapperListModel model.
        public static AttributeGroupMapperListModel ToGroupMapperListModel(IList<ZnodeMediaAttributeGroupMapper> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new AttributeGroupMapperListModel() { AttributeGroupMappers = new List<AttributeGroupMapperModel>() };
                foreach (ZnodeMediaAttributeGroupMapper item in entity)
                {
                    model.AttributeGroupMappers.Add(ToGroupMapperModel(item));
                }
                return model;
            }
            else
                return null;
        }

        //Convert ZnodeMediaAttributeGroupMapper entity in to AttributeFamilyModel model.
        public static AttributeGroupMapperModel ToGroupMapperModel(ZnodeMediaAttributeGroupMapper entity)
        {
            if (!Equals(entity, null))
            {
                return new AttributeGroupMapperModel
                {
                    MediaAttributeGroupMapperId = entity.MediaAttributeGroupMapperId,
                    MediaAttributeId = entity.MediaAttributeId,
                    MediaAttributeGroupId = entity.MediaAttributeGroupId
                };
            }
            else
                return null;
        }
    }
}
