using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class MediaAttributeValuesMap
    {
        //Converts View of MediaAttributeValues entity to AttributeGroupMapper model.
        public static MediaAttributeValuesModel ToModel(View_MediaAttributeValues entity)
        {
            if (HelperUtility.IsNotNull(entity))
            {
                return new MediaAttributeValuesModel
                {
                    MediaCategoryId = entity.MediaCategoryId,
                    AttributeCode = entity.AttributeCode,
                    AttributeFamilyId = entity.MediaAttributeFamilyId,
                    AttributeId = entity.MediaAttributeId,
                    AttributeName = entity.AttributeName,
                    AttributeTypeId = entity.AttributeTypeId,
                    DefaultAttributeValue = entity.DefaultAttributeValue,
                    DefaultAttributeValueId = entity.MediaAttributeDefaultValueId,
                    IsFilterable = entity.IsFilterable,
                    IsLocalizable = entity.IsLocalizable,
                    IsRequired = entity.IsRequired,
                    MediaAttributeValue = entity.AttributeValue,
                    MediaId = entity.MediaId.GetValueOrDefault(),
                    MediaPathId = entity.MediaPathId,
                    AttributeTypeName = entity.AttributeTypeName,
                    MediaAttributeValueId = entity.MediaAttributeValueId,
                    MediaPath = entity.MediaPath,
                    IsEditable = entity.IsEditable.GetValueOrDefault(),
                    ControlName = entity.ControlName,
                    ValidationName = entity.ValidationName,
                    SubValidationName = entity.SubValidationName,
                    RegExp = entity.RegExp,
                    ValidationValue = entity.ValidationValue,
                    IsRegExp = entity.IsRegExp,
                    AttributeGroupName = entity.AttributeGroupName,
                    FamilyCode = entity.FamilyCode,
                    HelpDescription = entity.HelpDescription,
                    MediaAttributeThumbnailPath=entity.MediaAttributeThumbnailPath
                };
            }

            return null;
        }

        //Converts List of View_MediaAttributeValues entities to MediaAttributeValuesListModel Model.
        public static MediaAttributeValuesListModel ToListModel(IEnumerable<View_MediaAttributeValues> entityList)
        {
            if (HelperUtility.IsNotNull(entityList))
            {
                var model = new MediaAttributeValuesListModel();

                foreach (var item in entityList)
                    model.MediateAttributeValues?.Add(ToModel(item));
                model.MediaVirtualPath = model.MediateAttributeValues?.Count > 0 ? model.MediateAttributeValues[0]?.MediaPath : string.Empty;
                return model;
            }
            else
                return null;
        }
    }
}
