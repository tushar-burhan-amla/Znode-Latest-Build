using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using static Znode.Engine.Admin.Helpers.HelperMethods;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Admin.Extensions;

namespace Znode.Engine.Admin.Maps
{
    public class AttributeGroupViewModelMap
    {
        // This method maps IEnumerable type list of AttributeGroupModel to SelectListItem type list of AttributeGroupViewModel.
        public static List<SelectListItem> ToListItems(IEnumerable<AttributeGroupModel> model)
        {
            List<SelectListItem> attributeGroupItem = new List<SelectListItem>();
            if (!Equals(model, null))
            {
                attributeGroupItem = (from item in model
                                      select new SelectListItem
                                      {
                                          Text = item.GroupCode,
                                          Value = item.MediaAttributeGroupId.ToString()
                                      }).ToList();
            }
            return attributeGroupItem;
        }

        public static AttributeGroupModel ToModel(BindDataModel model)
        {
            AttributeGroupModel attributeGroupModel = new AttributeGroupModel();
            if (!Equals(model, null))
            {
                attributeGroupModel.MediaAttributeGroupId = Convert.ToInt32(model.GetValue(ZnodeMediaAttributeGroupEnum.MediaAttributeGroupId.ToString()));
                attributeGroupModel.GroupCode = Convert.ToString(model.GetValue(ZnodeMediaAttributeGroupEnum.GroupCode.ToString()));
                attributeGroupModel.DisplayOrder =Equals(Convert.ToString(model.GetValue(ZnodeMediaAttributeEnum.DisplayOrder.ToString())), "") 
                                                    ? (int?)null : Convert.ToInt32(model.GetValue(ZnodeMediaAttributeEnum.DisplayOrder.ToString()));
                attributeGroupModel.IsSystemDefined = Equals(Convert.ToString(model.GetValue(ZnodeMediaAttributeGroupEnum.IsSystemDefined.ToString())), "")
                                        ? false : Convert.ToBoolean(model.GetValue(ZnodeMediaAttributeGroupEnum.IsSystemDefined.ToString()));
                attributeGroupModel.GroupLocaleListModel = ToLocaleListModel(model).AttributeGroupLocales;
            }
            return attributeGroupModel;
        }

        public static AttributeGroupLocaleListModel ToLocaleListModel(BindDataModel model)
        {
            var values = model.GetValue("LocaleLabel");
            int attributeGroupId = Convert.ToInt32(model.GetValue("MediaAttributeGroupId"));
            var list = (Convert.ToString(values)?.Split(',').ToList());
            var groupCode = Convert.ToString(model.GetValue("GroupCode"));

            AttributeGroupLocaleListModel _locales = new AttributeGroupLocaleListModel();
            if (list?.Count > 1)
            {
                _locales.AttributeGroupLocales = new List<AttributeGroupLocaleModel>();
                if (list.Count > 2)
                {
                    for (int i = 0; i < list.Count; i = i + 2)
                    {
                        _locales.AttributeGroupLocales.Add(new AttributeGroupLocaleModel()
                        {
                            LocaleId = Convert.ToInt32(list[i + 1]),
                            AttributeGroupName = list[i],
                            MediaAttributeGroupId = attributeGroupId,
                        });
                        _locales.GroupCode = groupCode;
                    }
                }
                else
                {
                    _locales.AttributeGroupLocales.Add(new AttributeGroupLocaleModel()
                    {
                        LocaleId = Convert.ToInt32(list[1]),
                        AttributeGroupName = list[0],
                        MediaAttributeGroupId = attributeGroupId
                    });
                    _locales.GroupCode = groupCode;
                }
            }

            return _locales;
        }
    
        public static List<LocaleDataModel> ToLocaleDataModel(LocaleListModel model, AttributeGroupLocaleListModel values)
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

        //This method maps AttributeGroupListModel to AttributeGroupListViewModel and returns AttributeGroupListViewModel.
        public static AttributeGroupListViewModel ToListViewModel(AttributeGroupListModel listModel)
        {
            if (listModel?.AttributeGroups?.Count > 0)
            {
                AttributeGroupListViewModel listViewModel = new AttributeGroupListViewModel()
                {
                    AttributeGroups = listModel.AttributeGroups.Select(
                    model => new AttributeGroupViewModel()
                    {
                        GroupCode = model.GroupCode,
                        AttributeGroupName = model.AttributeGroupName,
                        MediaAttributeGroupId = model.MediaAttributeGroupId,
                        AttributeModel = model.AttributeModel,
                        IsSystemDefined = model.IsSystemDefined,
                        CreatedDate= model.CreatedDate.ToDateTimeFormat(),
                        ModifiedDate= model.ModifiedDate.ToDateTimeFormat()
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
                return new AttributeGroupListViewModel();
            }
        }

        //This method maps AttributesListDataModel to List<BaseDropDownList>.
        public static List<BaseDropDownList> ToBaseDropDownList(AttributesListDataModel attributesList)
        {
            List<BaseDropDownList> unassignedAttributes = new List<BaseDropDownList>();
            if (attributesList?.Attributes?.Count > 0)
            {
                unassignedAttributes = attributesList.Attributes.Select(x => new BaseDropDownList
                {
                    id = x.MediaAttributeId.ToString(),
                    name = x.AttributeName,
                }).ToList();
            }
            return unassignedAttributes;
        }
    }
}