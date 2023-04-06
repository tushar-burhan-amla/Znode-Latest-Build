using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.AttributeValidationHelpers;
using static Znode.Engine.Admin.Helpers.HelperMethods;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Extensions;

namespace Znode.Engine.Admin.Maps
{
    public class AttributeFamilyViewModelMap
    {
        // This method maps IEnumerable type list of AttributeFamilyModel to SelectListItem type list of AttributeFamilyViewModel.
        public static List<SelectListItem> ToListItems(IEnumerable<AttributeFamilyModel> model)
        {
            List<SelectListItem> attributeFamilyItem = new List<SelectListItem>();
            if (!Equals(model, null))
            {
                attributeFamilyItem = (from item in model
                                       select new SelectListItem
                                       {
                                           Text = item.FamilyCode,
                                           Value = item.MediaAttributeFamilyId.ToString()
                                       }).ToList();
            }
            attributeFamilyItem.Insert(0, new SelectListItem() { Value = "0", Text = "Select Family" });
            return attributeFamilyItem;
        }


        //This method maps FamilyGroupAttributeListViewModel to FamilyGroupAttributeListModel and returns the FamilyGroupAttributeListModel Model.
        public static FamilyGroupAttributeListModel ToFamilyGroupAttributeListModel(List<string> attributeGroupIds, int attributeFamilyId, AttributeGroupMapperListModel attributes)
        {
            FamilyGroupAttributeListModel listModel = new FamilyGroupAttributeListModel() { FamilyGroupAttributes = new List<FamilyGroupAttributeModel>() };
            foreach (string item in attributeGroupIds)
            {
                var attributeList = attributes.AttributeGroupMappers.Where(y => y.MediaAttributeGroupId == Convert.ToInt32(item)).Select(p => p.MediaAttributeId).ToList();
                if (attributeList.Count == 0)
                {
                    listModel.FamilyGroupAttributes.Add(new FamilyGroupAttributeModel()
                    {
                        AttributeFamilyId = attributeFamilyId,
                        AttributeGroupId = Convert.ToInt32(item),
                    });
                }
                else
                {
                    foreach (var id in attributeList)
                    {
                        listModel.FamilyGroupAttributes.Add(new FamilyGroupAttributeModel()
                        {
                            AttributeFamilyId = attributeFamilyId,
                            AttributeGroupId = Convert.ToInt32(item),
                            AttributeId = id.Value
                        });
                    }
                }
            }
            return listModel;
        }

        //This method maps AttributeFamilyListModel to AttributeFamilyListViewModel.
        public static AttributeFamilyListViewModel ToListViewModel(AttributeFamilyListModel listModel)
        {
            if (listModel?.AttributeFamilies?.Count > 0)
            {
                return new AttributeFamilyListViewModel()
                {
                    AttributeFamilies = (from _item in listModel.AttributeFamilies
                                         select new AttributeFamilyViewModel
                                         {
                                             FamilyCode = _item.FamilyCode,
                                             MediaAttributeFamilyId = _item.MediaAttributeFamilyId,
                                             IsDefaultFamily = _item.IsDefaultFamily,
                                             CreatedDate = _item.CreatedDate.ToDateTimeFormat(),
                                             ModifiedDate = _item.ModifiedDate.ToDateTimeFormat(),
                                             IsSystemDefined = _item.IsSystemDefined,
                                             AttributeFamilyName=_item.AttributeFamilyName
                                             
                                         }).ToList(),
                    Page = Convert.ToInt32(listModel.PageIndex),
                    RecordPerPage = Convert.ToInt32(listModel.PageSize),
                    TotalPages = Convert.ToInt32(listModel.TotalPages),
                    TotalResults = Convert.ToInt32(listModel.TotalResults)
                };
            }
            return null;
        }

        //Mapping Of LocalelistModel of family To LocaleDataModel
        public static List<LocaleDataModel> ToLocaleDataModel(LocaleListModel model, FamilyLocaleListModel values)
        {
            return (from _list in model.Locales
                    select new LocaleDataModel
                    {
                        LocaleId = _list.LocaleId,
                        Name = _list.Name,
                        Code = _list.Code,
                        IsActive = _list.IsActive,
                        IsDefault = _list.IsDefault,
                        Value = Equals(values, null) ? null : !Equals(values.FamilyLocales.FirstOrDefault(x => x.LocaleId == _list.LocaleId), null) ? values.FamilyLocales.FirstOrDefault(x => x.LocaleId == _list.LocaleId).AttributeFamilyName : null
                    }).ToList();
        }

        //This method maps BindDataModel to FamilyLocaleListModel.
        public static FamilyLocaleListModel ToFamilyLocaleListModel(BindDataModel model)
        {
            var values = model.GetValue("LocaleLabel");
            int attributeFamilyId = Convert.ToInt32(model.GetValue("MediaAttributeFamilyId"));
            var list = (Convert.ToString(values)?.Split(',').ToList());
            string FamilyCode = Convert.ToString(model.GetValue("FamilyCode"));

            FamilyLocaleListModel _locales = new FamilyLocaleListModel();
            if (list?.Count > 0)
            {
                _locales.FamilyLocales = new List<FamilyLocaleModel>();
                if (list.Count > 2)
                {
                    for (int i = 0; i < list.Count; i = i + 2)
                    {
                        _locales.FamilyLocales.Add(new FamilyLocaleModel()
                        {
                            LocaleId = Convert.ToInt32(list[i + 1]),
                            AttributeFamilyName = list[i],
                            AttributeFamilyId = attributeFamilyId
                        });
                    }
                    _locales.FamilyCode = FamilyCode;
                }
                else
                {
                    _locales.FamilyLocales.Add(new FamilyLocaleModel()
                    {
                        LocaleId = Convert.ToInt32(list[1]),
                        AttributeFamilyName = list[0],
                        AttributeFamilyId = attributeFamilyId
                    });
                    _locales.FamilyCode = FamilyCode;
                }
            }

            return _locales;
        }

       
        //This method maps AttributeGroupListModel to List<BaseDropDownList>.
        public static List<BaseDropDownList> ToBaseDropDownList(AttributeGroupListModel attributeGroupList)
        {
            List<BaseDropDownList> unassignedAttributeGroups = new List<BaseDropDownList>();
            if (attributeGroupList?.AttributeGroups?.Count > 0)
            {
                unassignedAttributeGroups = attributeGroupList.AttributeGroups.Select(x => new BaseDropDownList
                {
                    id = x.MediaAttributeGroupId.ToString(),
                    name = x.AttributeGroupName,
                }).ToList();
            }
            return unassignedAttributeGroups;
        }
    }
}