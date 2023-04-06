using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Extensions;

namespace Znode.Engine.Admin.Maps
{
    public static class MediaAttributeValuesViewModelMap
    {
        public static MediaAttributeValuesViewModel ToViewModel(MediaAttributeValuesModel model)
        {
            if (Equals(model, null))
                return new MediaAttributeValuesViewModel();
            return new MediaAttributeValuesViewModel()
            {
                MediaCategoryId = model.MediaCategoryId,
                MediaPathId = model.MediaPathId,
                MediaId = model.MediaId,
                AttributeFamilyId = model.AttributeFamilyId,
                AttributeId = model.AttributeId,
                AttributeTypeId = model.AttributeTypeId,
                AttributeCode = model.AttributeCode,
                IsRequired = model.IsRequired,
                IsLocalizable = model.IsLocalizable,
                IsFilterable = model.IsFilterable,
                AttributeName = model.AttributeName,
                MediaAttributeValue = model.MediaAttributeValue,
                DefaultAttributeValueId = model.DefaultAttributeValueId,
                DefaultAttributeValue = model.DefaultAttributeValue,
                MediaAttributeValueId = model.MediaAttributeValueId,
                MediaPath = model.MediaPath,
                AttributeTypeName = model.AttributeTypeName,
                IsEditable = model.IsEditable.GetValueOrDefault(),
                ControlName = model.ControlName,
                ValidationName = model.ValidationName,
                SubValidationName = model.SubValidationName,
                RegExp = model.RegExp,
                ValidationValue = model.ValidationValue,
                IsRegExp = model.IsRegExp,
                AttributeGroupName = model.AttributeGroupName,
                FamilyCode = model.FamilyCode,
                HelpDescription = model.HelpDescription,
                MediaAttributeThumbnailPath=model.MediaAttributeThumbnailPath
            };
        }

        public static MediaAttributeValuesListViewModel ToListViewModel(MediaAttributeValuesListModel listModel)
        {
            if (!Equals(listModel, null))
            {
                MediaAttributeValuesListViewModel listViewModel = new MediaAttributeValuesListViewModel();
                listViewModel.FileName = listModel.Media.FileName;
                listViewModel.Size = listModel.Media.Size.ToDisplayUnitFormat();
                listViewModel.Height = !string.IsNullOrEmpty(listModel.Media.Height) ? listModel.Media.Height.Trim() + " pixels" : listModel.Media.Height;
                listViewModel.Width = !string.IsNullOrEmpty(listModel.Media.Width) ? listModel.Media.Width.Trim() + " pixels" : listModel.Media.Width;
                listViewModel.Length = listModel.Media.Length;
                listViewModel.Type = listModel.Media.MediaType;
                listViewModel.CreatedDate = Convert.ToString(listModel.Media.CreatedDate.ToDateTimeFormat());
                listViewModel.ModifiedDate = Convert.ToString(listModel.Media.ModifiedDate.ToDateTimeFormat());
                listViewModel.MediaId = listModel.Media.MediaId;
                listViewModel.MediaPathId = listModel.Media.MediaPathId;
                if (listModel?.MediateAttributeValues?.Count > 0)
                {
                    foreach (var model in listModel.MediateAttributeValues)
                        listViewModel.MediaAttributeValues.Add(ToViewModel(model));
                    listViewModel.FamilyCode = listViewModel?.MediaAttributeValues[0]?.FamilyCode ?? null;
                    listViewModel.MediaVirtualPath = listViewModel.MediaAttributeValues[0].MediaPath ?? string.Empty;
                }
                return listViewModel;
            }
            return new MediaAttributeValuesListViewModel();
        }

        //Appended keys with property name{AttributeCode}[0]_{mediaAttributeValueId}[1]_{DefaultAttributeValueId}[2]_{AttributeId}[3]_{MediaCategoryId}[4].
        public static MediaAttributeValuesListModel ToListModel(BindDataModel model, MediaManagerViewModel metaInfo)
        {
            MediaAttributeValuesListModel listModel = new MediaAttributeValuesListModel();
            listModel.Media = new MediaManagerModel();
            listModel.Media.FileName = metaInfo.FileName;
            listModel.Media.MediaId = metaInfo.MediaId;
            model.ControlsData.ToList().ForEach(item =>
            {
                List<object> itemList = new List<object>();
                itemList.AddRange(item.Key.Split('_'));
                listModel.MediateAttributeValues.Add(new MediaAttributeValuesModel
                {
                    MediaAttributeValueId = Convert.ToInt32(itemList[1]),
                    MediaCategoryId = Convert.ToInt32(itemList[4]),
                    AttributeId = Equals(itemList[3], null) ? (int?)itemList[3] : Convert.ToInt32(itemList[3]),
                    AttributeCode = itemList[0].ToString(),
                    DefaultAttributeValueId = Equals(itemList[2], null) ? (int?)itemList[2] : Convert.ToInt32(itemList[2]),
                    MediaAttributeValue = item.Value.ToString()
                });
            });
            return listModel;
        }
    }
}