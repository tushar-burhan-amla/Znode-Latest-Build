using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin
{
    public static class GlobalAttributeGroupViewModelMap
    {
        public static List<LocaleDataModel> ToLocaleDataModel(LocaleListModel model, GlobalAttributeGroupLocaleListModel values)
        {
            return (from _list in model.Locales
                    select new LocaleDataModel
                    {
                        LocaleId = _list.LocaleId,
                        Name = _list.Name,
                        Code = _list.Code,
                        IsActive = _list.IsActive,
                        IsDefault = _list.IsDefault,
                        Value = (IsNull(values) || IsNull(values.AttributeGroupLocales)) ? null : IsNotNull(values.AttributeGroupLocales?.FirstOrDefault(x => x.LocaleId == _list.LocaleId)) ? values.AttributeGroupLocales.FirstOrDefault(x => x.LocaleId == _list.LocaleId).AttributeGroupName : null
                    }).ToList();
        }

        //Maps bind data model to global attribute group model.
        public static GlobalAttributeGroupModel ToModel(BindDataModel model)
        {
            GlobalAttributeGroupModel globalAttributeGroupModel = new GlobalAttributeGroupModel();
            if (IsNotNull(model))
            {
                globalAttributeGroupModel.GlobalAttributeGroupId = Convert.ToInt32(model.GetValue(ZnodeGlobalAttributeGroupEnum.GlobalAttributeGroupId.ToString()));
                globalAttributeGroupModel.GroupCode = Convert.ToString(model.GetValue(ZnodeGlobalAttributeGroupEnum.GroupCode.ToString()));

                globalAttributeGroupModel.AttributeGroupLocales = ToLocaleListModel(model).AttributeGroupLocales;
                globalAttributeGroupModel.GlobalEntityId = Convert.ToInt32(model.GetValue("GlobalEntityId"));
            }
            return globalAttributeGroupModel;
        }

        //Maps bind data model to global attribute group locale list model.
        public static GlobalAttributeGroupLocaleListModel ToLocaleListModel(BindDataModel model)
        {
            var values = model.GetValue("LocaleLabel");
            int globalAttributeGroupId = Convert.ToInt32(model.GetValue("GlobalAttributeGroupId"));
            var list = (Convert.ToString(values)?.Split(',').ToList());
            var groupCode = Convert.ToString(model.GetValue("GroupCode"));

            GlobalAttributeGroupLocaleListModel _locales = new GlobalAttributeGroupLocaleListModel();
            if (list?.Count > 1)
            {
                _locales.AttributeGroupLocales = new List<GlobalAttributeGroupLocaleModel>();
                if (list.Count > 2)
                {
                    for (int i = 0; i < list.Count; i = i + 2)
                    {
                        _locales.AttributeGroupLocales.Add(new GlobalAttributeGroupLocaleModel()
                        {
                            LocaleId = Convert.ToInt32(list[i + 1]),
                            AttributeGroupName = list[i],
                            GlobalAttributeGroupId = globalAttributeGroupId,
                        });
                        _locales.GroupCode = groupCode;
                    }
                }
                else
                {
                    _locales.AttributeGroupLocales.Add(new GlobalAttributeGroupLocaleModel()
                    {
                        LocaleId = Convert.ToInt32(list[1]),
                        AttributeGroupName = list[0],
                        GlobalAttributeGroupId = globalAttributeGroupId
                    });
                    _locales.GroupCode = groupCode;
                }
            }

            return _locales;
        }

        //Maps Global attribute list model into base drop down list model.
        public static List<BaseDropDownList> ToBaseDropDownList(GlobalAttributeListModel attributesList)
        {
            List<BaseDropDownList> unassignedAttributes = new List<BaseDropDownList>();
            if (attributesList?.Attributes?.Count > 0)
            {
                unassignedAttributes = attributesList.Attributes.Select(x => new BaseDropDownList
                {
                    id = x.GlobalAttributeId.ToString(),
                    name = x.AttributeName,
                }).ToList();
            }
            return unassignedAttributes;
        }

        public static GlobalAttributeGroupMapperListModel ToAttributeGroupMapperListModel(List<string> attributeGroupIds, int attributeGroupId)
        {
            GlobalAttributeGroupMapperListModel listModel = new GlobalAttributeGroupMapperListModel() { AttributeGroupMappers = new List<GlobalAttributeGroupMapperModel>() };
            foreach (string item in attributeGroupIds)
            {
                listModel.AttributeGroupMappers.Add(new GlobalAttributeGroupMapperModel()
                {
                    GlobalAttributeGroupId = attributeGroupId,
                    GlobalAttributeId = Convert.ToInt32(item)
                });
            }
            return listModel;
        }

        //Convert attribute list model to attribute list view model
        public static GlobalAttributeGroupMapperListViewModel ToListViewModel(GlobalAttributeGroupMapperListModel attributeListModel)
        {
            GlobalAttributeGroupMapperListViewModel listViewModel = new GlobalAttributeGroupMapperListViewModel();
            if (IsNotNull(attributeListModel))
            {
                listViewModel.AttributeGroupMappers = new List<GlobalAttributeGroupMapperViewModel>();
                if (attributeListModel?.AttributeGroupMappers?.Count > 0)
                {
                    foreach (GlobalAttributeGroupMapperModel model in attributeListModel.AttributeGroupMappers)
                    {
                        //Maps the API attributes list model with  attributes list model.
                        listViewModel.AttributeGroupMappers.Add(ToViewModel(model, model.Attribute));
                    }
                    //set for pagination
                    listViewModel.Page = Convert.ToInt32(attributeListModel.PageIndex);
                    listViewModel.RecordPerPage = Convert.ToInt32(attributeListModel.PageSize);
                    listViewModel.TotalPages = Convert.ToInt32(attributeListModel.TotalPages);
                    listViewModel.TotalResults = Convert.ToInt32(attributeListModel.TotalResults);              
                }
            }
            return listViewModel;
        }

        //Maps global attribute model data into mapper model.
        public static GlobalAttributeGroupMapperViewModel ToViewModel(GlobalAttributeGroupMapperModel model, GlobalAttributeModel attribute)
        {
            GlobalAttributeGroupMapperViewModel globalAttributeGroupModel = new GlobalAttributeGroupMapperViewModel();
            if (IsNotNull(model))
            {
                globalAttributeGroupModel.GlobalAttributeGroupId = model.GlobalAttributeGroupId;
                globalAttributeGroupModel.GlobalAttributeGroupMapperId = model.GlobalAttributeGroupMapperId;
                globalAttributeGroupModel.GlobalAttributeId = model.GlobalAttributeId;

                if (IsNotNull(attribute))
                {
                    globalAttributeGroupModel.AttributeCode = attribute.AttributeCode;
                    globalAttributeGroupModel.IsRequired = attribute.IsRequired;
                    globalAttributeGroupModel.AttributeType = attribute.AttributeType;
                    globalAttributeGroupModel.IsLocalizable = IsNotNull(attribute.IsLocalizable) ? attribute.IsLocalizable : false;
                    globalAttributeGroupModel.AttributeName = attribute.AttributeName;
                    globalAttributeGroupModel.DisplayOrder = attribute.DisplayOrder;
                }
            }
            return globalAttributeGroupModel;
        }
    }
}
