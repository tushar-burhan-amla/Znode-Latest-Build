using System;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.ViewModels;
using System.Collections.Generic;

namespace Znode.Engine.Admin.Maps
{
    public static class AttributeGroupMapperViewModelMap
    {
        public static AttributeGroupMapperListViewModel ToListViewModel(AttributeGroupMapperListModel listModel)
        {
            AttributeGroupMapperListViewModel listViewModel = new AttributeGroupMapperListViewModel();
            if (!Equals(listModel, null))
            {
                listViewModel.AttributeGroupMappers = new List<AttributeGroupMapperViewModel>();
                if (listModel?.AttributeGroupMappers?.Count > 0)
                {
                    foreach (AttributeGroupMapperModel model in listModel.AttributeGroupMappers)
                    {
                        //Maps the API attributes list model with  attributes list model.
                        listViewModel.AttributeGroupMappers.Add(ToViewModel(model, model.Attribute));
                    }
                    //set for pagination
                    listViewModel.Page = Convert.ToInt32(listModel.PageIndex);
                    listViewModel.RecordPerPage = Convert.ToInt32(listModel.PageSize);
                    listViewModel.TotalPages = Convert.ToInt32(listModel.TotalPages);
                    listViewModel.TotalResults = Convert.ToInt32(listModel.TotalResults);
                }
            }
            return listViewModel;
        }

        public static AttributeGroupMapperViewModel ToViewModel(AttributeGroupMapperModel model, AttributesDataModel attribute)
        {
            AttributeGroupMapperViewModel attributeGroupModel = new AttributeGroupMapperViewModel();
            if (!Equals(model, null))
            {
                attributeGroupModel.MediaAttributeGroupId = model.MediaAttributeGroupId;
                attributeGroupModel.MediaAttributeGroupMapperId = model.MediaAttributeGroupMapperId;
                attributeGroupModel.MediaAttributeId = model.MediaAttributeId;
                attributeGroupModel.IsSystemDefined = model.IsSystemDefined;
                if (!Equals(attribute, null))
                {
                    attributeGroupModel.Code = attribute.AttributeCode;
                    attributeGroupModel.IsRequired = attribute.IsRequired.GetValueOrDefault();
                    attributeGroupModel.AttributeType = attribute.AttributeTypeName;
                    attributeGroupModel.IsLocalizable = Convert.ToBoolean(attribute.IsLocalizable);
                }
            }
            return attributeGroupModel;
        }

        public static AttributeGroupMapperListModel ToAttributeGroupMapperListModel(List<string> attributeGroupIds, int attributeGroupId)
        {
            AttributeGroupMapperListModel listModel = new AttributeGroupMapperListModel() { AttributeGroupMappers = new List<AttributeGroupMapperModel>() };
            foreach (string item in attributeGroupIds)
            {
                listModel.AttributeGroupMappers.Add(new AttributeGroupMapperModel()
                {
                    MediaAttributeGroupId = attributeGroupId,
                    MediaAttributeId = Convert.ToInt32(item)
                });
            }
            return listModel;
        }
    }
}