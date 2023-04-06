using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Engine.Admin.Helpers.HelperMethods;

namespace Znode.Engine.Admin
{
    public static class PIMAttributeGroupViewModelMap
    {
        //This method maps PIMAttributeGroupListModel to AttributeGroupListViewModel and returns PIMAttributeGroupListViewModel.
        public static PIMAttributeGroupListViewModel ToListViewModel(PIMAttributeGroupListModel listModel)
        {
            if (listModel?.AttributeGroupList?.Count > 0)
            {
                PIMAttributeGroupListViewModel listViewModel = new PIMAttributeGroupListViewModel()
                {
                    AttributeGroupList = listModel.AttributeGroupList.Select(
                    model => new PIMAttributeGroupViewModel()
                    {
                        GroupCode = model.GroupCode,
                        PimAttributeGroupId = model.PimAttributeGroupId,
                        Attributes = model.Attributes,
                        IsSystemDefined = model.IsSystemDefined,
                        DisplayOrder = model.DisplayOrder,
                        ModifiedDate = model.ModifiedDate.ToDateTimeFormat(),
                        CreatedDate = model.CreatedDate.ToDateTimeFormat(),
                        AttributeGroupName = model.AttributeGroupName
                    }).ToList()
                };
                listViewModel.Page = Convert.ToInt32(listModel.PageIndex);
                listViewModel.RecordPerPage = Convert.ToInt32(listModel.PageSize);
                listViewModel.TotalPages = Convert.ToInt32(listModel.TotalPages);
                listViewModel.TotalResults = Convert.ToInt32(listModel.TotalResults);
                return listViewModel;
            }
            else
            {
                return new PIMAttributeGroupListViewModel();
            }
        }

        public static List<LocaleDataModel> ToLocaleDataModel(LocaleListModel model, PIMAttributeGroupLocaleListModel values)
        {
            return (from _list in model.Locales
                    select new LocaleDataModel
                    {
                        LocaleId = _list.LocaleId,
                        Name = _list.Name,
                        Code = _list.Code,
                        IsActive = _list.IsActive,
                        IsDefault = _list.IsDefault,
                        Value = (Equals(values, null) || Equals(values.AttributeGroupLocales, null)) ? null : !Equals(values.AttributeGroupLocales?.FirstOrDefault(x => x.LocaleId == _list.LocaleId), null) ? values.AttributeGroupLocales.FirstOrDefault(x => x.LocaleId == _list.LocaleId).AttributeGroupName : null
                    }).ToList();
        }

        //Convert Attributelistmodel to attributelistviewmodel
        public static PIMAttributeGroupMapperListViewModel ToListViewModel(PIMAttributeGroupMapperListModel attributeListModel)
        {
            PIMAttributeGroupMapperListViewModel listViewModel = new PIMAttributeGroupMapperListViewModel();
            if (!Equals(attributeListModel, null))
            {
                listViewModel.AttributeGroupMappers = new List<PIMAttributeGroupMapperViewModel>();
                if (attributeListModel?.AttributeGroupMappers?.Count > 0)
                {
                    foreach (PIMAttributeGroupMapperModel model in attributeListModel.AttributeGroupMappers)
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

        public static PIMAttributeGroupLocaleListModel ToLocaleListModel(BindDataModel model)
        {
            var values = model.GetValue("LocaleLabel");
            int pimAttributeGroupId = Convert.ToInt32(model.GetValue("PimAttributeGroupId"));
            var list = (Convert.ToString(values)?.Split(',').ToList());
            var groupCode = Convert.ToString(model.GetValue("GroupCode"));

            PIMAttributeGroupLocaleListModel _locales = new PIMAttributeGroupLocaleListModel();
            if (list?.Count > 1)
            {
                _locales.AttributeGroupLocales = new List<PIMAttributeGroupLocaleModel>();
                if (list.Count > 2)
                {
                    for (int i = 0; i < list.Count; i = i + 2)
                    {
                        _locales.AttributeGroupLocales.Add(new PIMAttributeGroupLocaleModel()
                        {
                            LocaleId = Convert.ToInt32(list[i + 1]),
                            AttributeGroupName = list[i],
                            PIMAttributeGroupId = pimAttributeGroupId,
                        });
                        _locales.GroupCode = groupCode;
                    }
                }
                else
                {
                    _locales.AttributeGroupLocales.Add(new PIMAttributeGroupLocaleModel()
                    {
                        LocaleId = Convert.ToInt32(list[1]),
                        AttributeGroupName = list[0],
                        PIMAttributeGroupId = pimAttributeGroupId
                    });
                    _locales.GroupCode = groupCode;
                }
            }

            return _locales;
        }

        public static PIMAttributeGroupModel ToModel(BindDataModel model)
        {
            PIMAttributeGroupModel pimAttributeGroupModel = new PIMAttributeGroupModel();
            if (!Equals(model, null))
            {
                pimAttributeGroupModel.PimAttributeGroupId = Convert.ToInt32(model.GetValue(ZnodePimAttributeGroupEnum.PimAttributeGroupId.ToString()));
                pimAttributeGroupModel.GroupCode = Convert.ToString(model.GetValue(ZnodePimAttributeGroupEnum.GroupCode.ToString()));
                pimAttributeGroupModel.IsSystemDefined = Convert.ToBoolean(model.GetValue(ZnodePimAttributeGroupEnum.IsSystemDefined.ToString()));
                pimAttributeGroupModel.IsCategory = Convert.ToBoolean(model.GetValue(ZnodePimAttributeGroupEnum.IsCategory.ToString()));
                pimAttributeGroupModel.IsNonEditable = Convert.ToBoolean(model.GetValue(ZnodePimAttributeGroupEnum.IsNonEditable.ToString()));
                pimAttributeGroupModel.AttributeGroupLocales = ToLocaleListModel(model).AttributeGroupLocales;
            }
            return pimAttributeGroupModel;
        }

        public static PIMAttributeGroupMapperListModel ToAttributeGroupMapperListModel(List<string> attributeGroupIds, int attributeGroupId)
        {
            PIMAttributeGroupMapperListModel listModel = new PIMAttributeGroupMapperListModel() { AttributeGroupMappers = new List<PIMAttributeGroupMapperModel>() };
            foreach (string item in attributeGroupIds)
            {
                listModel.AttributeGroupMappers.Add(new PIMAttributeGroupMapperModel()
                {
                    PimAttributeGroupId = attributeGroupId,
                    PimAttributeId = Convert.ToInt32(item)
                });
            }
            return listModel;
        }

        public static PIMAttributeGroupMapperViewModel ToViewModel(PIMAttributeGroupMapperModel model, PIMAttributeModel attribute)
        {
            PIMAttributeGroupMapperViewModel pimAttributeGroupModel = new PIMAttributeGroupMapperViewModel();
            if (!Equals(model, null))
            {
                pimAttributeGroupModel.PimAttributeGroupId = model.PimAttributeGroupId;
                pimAttributeGroupModel.PimAttributeGroupMapperId = model.PimAttributeGroupMapperId;
                pimAttributeGroupModel.PimAttributeId = model.PimAttributeId;
                pimAttributeGroupModel.IsSystemDefined = model.IsSystemDefined;

                if (!Equals(attribute, null))
                {
                    pimAttributeGroupModel.AttributeCode = attribute.AttributeCode;
                    pimAttributeGroupModel.IsRequired = attribute.IsRequired;
                    pimAttributeGroupModel.AttributeType = attribute.AttributeType;
                    pimAttributeGroupModel.IsLocalizable = !Equals(attribute.IsLocalizable, null) ? attribute.IsLocalizable : false;
                    pimAttributeGroupModel.AttributeName = attribute.AttributeName;
                    pimAttributeGroupModel.DisplayOrder = attribute.DisplayOrder;
                }
            }
            return pimAttributeGroupModel;
        }

        public static List<BaseDropDownList> ToBaseDropDownList(PIMAttributeListModel attributesList)
        {
            List<BaseDropDownList> unassignedAttributes = new List<BaseDropDownList>();
            if (attributesList?.Attributes?.Count > 0)
            {
                unassignedAttributes = attributesList.Attributes.Select(x => new BaseDropDownList
                {
                    id = x.PimAttributeId.ToString(),
                    name = x.AttributeName,
                }).ToList();
            }
            return unassignedAttributes;
        }
    }
}