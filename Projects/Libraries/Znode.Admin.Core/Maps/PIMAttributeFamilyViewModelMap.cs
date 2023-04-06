using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Extensions;
using Znode.Libraries.Resources;
using Znode.Libraries.ECommerce.Utilities;
using System.Text.RegularExpressions;

namespace Znode.Engine.Admin
{
    public static class PIMAttributeFamilyViewModelMap
    {
        // This method maps IEnumerable type list of PIMAttributeFamilyModel to SelectListItem type list of PIMAttributeFamilyViewModel.
        public static List<SelectListItem> ToListItems(IEnumerable<PIMAttributeFamilyModel> model)
        {
            List<SelectListItem> attributeFamilyItem = new List<SelectListItem>();
            if (!Equals(model, null))
            {
                attributeFamilyItem = (from item in model
                                       orderby item.PimAttributeFamilyId ascending
                                       select new SelectListItem
                                       {
                                           Text = item.FamilyCode,
                                           Value = item.PimAttributeFamilyId.ToString()
                                       }).ToList();
            }
            return attributeFamilyItem;
        }

        // This method maps IEnumerable type list of LocaleModel to SelectListItem type list of LocaleModel.
        public static List<SelectListItem> ToLocaleListItem(IEnumerable<LocaleModel> model)
        {
            List<SelectListItem> LocaleItem = new List<SelectListItem>();
            if (!Equals(model, null))
            {
                LocaleItem = (from item in model
                              select new SelectListItem
                              {
                                  Text = item.Name,
                                  Value = item.LocaleId.ToString(),
                                  Selected = item.IsDefault
                              }).ToList();
            }
            return LocaleItem;
        }

        // This method maps PIMFamilyDetailsModel to PIMFamilyDetailsViewModel.
        public static PIMFamilyDetailsViewModel ToPIMFamilyDetailsViewModel(PIMFamilyDetailsModel model)
        {
            if (!Equals(model, null) && !Equals(model.Attributes, null) && !Equals(model.Groups, null) && !Equals(model.Family, null))
            {
                var familiesSelectList = (from item in model.Family
                                          orderby item.PimAttributeFamilyId ascending
                                          select new SelectListItem
                                          {
                                              Text = item.AttributeFamilyName,
                                              Value = item.PimAttributeFamilyId.ToString()
                                          }).ToList();
                //Locale List
                var LocaleSelectList = (from item in model.Locale
                                        select new SelectListItem
                                        {
                                            Text = item.Name,
                                            Value = item.LocaleId.ToString(),
                                            Selected = item.IsDefault
                                        }).ToList();

                PIMFamilyDetailsViewModel pimFamilyDetailsViewModel = new PIMFamilyDetailsViewModel
                {
                    Groups = model.Groups.ToViewModel<PIMAttributeGroupViewModel>().ToList(),
                    Attributes = model.Attributes.ToViewModel<PIMProductAttributeValuesViewModel>().ToList(),
                    Families = familiesSelectList,
                    FamilyId = model.PimAttributeFamilyId,
                    CategoryId = model.Id,
                    Name = model.Name,
                    SKU = model.SKU,
                    AssociatedProductIds = model.AssociatedProductIds,
                    ConfigureAttributeIds = model.ConfigureAttributeIds,
                    Locale = LocaleSelectList,
                    ProductPublishId = model.ProductPublishId
                };

                List<string> distinctAttributeCodes = pimFamilyDetailsViewModel.Attributes.Where(x => x != null).Select(e => e.AttributeCode + e.PimAttributeFamilyId).Distinct().ToList();
                pimFamilyDetailsViewModel.Attributes = GetAttributeControls(pimFamilyDetailsViewModel.Attributes, distinctAttributeCodes);
                pimFamilyDetailsViewModel.AssociatedProductIds = model.AssociatedProductIds;
                pimFamilyDetailsViewModel.ConfigureAttributeIds = model.ConfigureAttributeIds;
                pimFamilyDetailsViewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);

                return pimFamilyDetailsViewModel;
            }
            return null;
        }

        //GEt Value family view model from bind data model.
        public static PIMAttributeFamilyViewModel ToViewModel(BindDataModel model)
        {
            if (!Equals(model, null))
            {
                return new PIMAttributeFamilyViewModel()
                {
                    FamilyCode = model.GetValue(ZnodePimAttributeFamilyEnum.FamilyCode.ToString()).ToString(),
                    PimAttributeFamilyId = Convert.ToInt32(model.GetValue(ZnodePimAttributeFamilyEnum.PimAttributeFamilyId.ToString())),
                    ExistingAttributeFamilyId = Convert.ToInt32(model.GetValue("ExistingAttributeFamilyId")),
                    IsCategory = Convert.ToBoolean(model.GetValue(ZnodePimAttributeEnum.IsCategory.ToString())),
                    IsSystemDefined = Equals(Convert.ToString(model.GetValue(ZnodeMediaAttributeEnum.IsSystemDefined.ToString())), "")
                                        ? false : Convert.ToBoolean(model.GetValue(ZnodeMediaAttributeEnum.IsSystemDefined.ToString())),
                };
            }
            return null;
        }


        // This method maps PIMFamilyDetailsModel to PIMFamilyDetailsViewModel.
        public static PIMFamilyDetailsViewModel ToPIMConfigureDetailsViewModel(PIMFamilyDetailsModel model)
        {
            if (!Equals(model, null) && !Equals(model.Attributes, null))
            {
                PIMFamilyDetailsViewModel pimFamilyDetailsViewModel = new PIMFamilyDetailsViewModel
                {
                    //Groups = model.Groups.ToViewModel<PIMAttributeGroupViewModel>().ToList(),
                    Attributes = model.Attributes.ToViewModel<PIMProductAttributeValuesViewModel>().ToList(),
                    //Families = familiesSelectList,
                    FamilyId = model.PimAttributeFamilyId,
                };
                List<string> distinctAttributeCodes = pimFamilyDetailsViewModel.Attributes.Select(e => e.AttributeCode + e.PimAttributeFamilyId).Distinct().ToList();
                pimFamilyDetailsViewModel.Attributes = GetAttributeControls(pimFamilyDetailsViewModel.Attributes, distinctAttributeCodes);

                return pimFamilyDetailsViewModel;
            }
            return null;
        }

        //This method returns true if key is not present in dictionary else return false.
        public static bool IsKeyNotPresent(string key, IDictionary<string, object> source)
        {
            if (!Equals(source, null) && !string.IsNullOrEmpty(key))
                return !source.ContainsKey(key);
            return false;
        }



        //This method map PIMAttributeFamilyListModel to PIMAttributeFamilyListViewModel and return PIMAttributeFamilyListViewModel ListViewModel.
        public static PIMAttributeFamilyListViewModel ToListViewModel(PIMAttributeFamilyListModel listModel)
        {
            if (listModel?.PIMAttributeFamilies?.Count > 0)
            {
                return new PIMAttributeFamilyListViewModel()
                {
                    List = (from _item in listModel.PIMAttributeFamilies
                            select new PIMAttributeFamilyViewModel
                            {
                                FamilyCode = _item.FamilyCode,
                                PimAttributeFamilyId = _item.PimAttributeFamilyId,
                                IsDefaultFamily = _item.IsDefaultFamily,
                                CreatedDate = _item.CreatedDate.ToDateTimeFormat(),
                                ModifiedDate = _item.ModifiedDate.ToDateTimeFormat(),
                                IsSystemDefined = _item.IsSystemDefined,
                                AttributeFamilyName = _item.AttributeFamilyName
                            }).ToList(),
                    Page = Convert.ToInt32(listModel.PageIndex),
                    RecordPerPage = Convert.ToInt32(listModel.PageSize),
                    TotalPages = Convert.ToInt32(listModel.TotalPages),
                    TotalResults = Convert.ToInt32(listModel.TotalResults)
                };
            }
            return null;
        }

        //This method maps PIMFamilyGroupAttributeListViewModel to PIMFamilyGroupAttributeListModel and returns the PIMFamilyGroupAttributeListModel Model.
        public static PIMFamilyGroupAttributeListModel ToFamilyGroupAttributeListModel(List<string> attributeGroupIds, int attributeFamilyId, PIMAttributeGroupMapperListModel attributes)
        {
            PIMFamilyGroupAttributeListModel listModel = new PIMFamilyGroupAttributeListModel() { FamilyAttributeGroups = new List<PIMFamilyGroupAttributeModel>() };
            foreach (string item in attributeGroupIds)
            {
                var attributeList = attributes.AttributeGroupMappers.Where(y => y.PimAttributeGroupId == Convert.ToInt32(item)).Select(p => p.PimAttributeId).ToList();
                if (attributeList.Count == 0)
                {
                    listModel.FamilyAttributeGroups.Add(new PIMFamilyGroupAttributeModel()
                    {
                        AttributeFamilyId = attributeFamilyId,
                        AttributeGroupId = Convert.ToInt32(item),
                    });
                }
                else
                {
                    foreach (var id in attributeList)
                    {
                        listModel.FamilyAttributeGroups.Add(new PIMFamilyGroupAttributeModel()
                        {
                            AttributeFamilyId = attributeFamilyId,
                            AttributeGroupId = Convert.ToInt32(item),
                            AttributeId = id,
                        });
                    }
                }
            }
            return listModel;
        }

        //Mapping Of LocalelistModel of family To LocaleDataModel
        public static List<LocaleDataModel> ToLocaleDataModel(LocaleListModel model, PIMFamilyLocaleListModel values)
        {
            return (from _list in model.Locales
                    select new LocaleDataModel
                    {
                        LocaleId = _list.LocaleId,
                        Name = _list.Name,
                        Code = _list.Code,
                        IsActive = _list.IsActive,
                        IsDefault = _list.IsDefault,
                        Value = Equals(values, null) ? null : !Equals(values?.FamilyLocales?.FirstOrDefault(x => x.LocaleId == _list.LocaleId), null) ? values?.FamilyLocales?.FirstOrDefault(x => x.LocaleId == _list.LocaleId).AttributeFamilyName : null
                    }).ToList();
        }

        //This method maps BindDataModel to PIMFamilyLocaleListModel.
        public static PIMFamilyLocaleListModel ToFamilyLocaleListModel(BindDataModel model)
        {
            var values = model.GetValue("LocaleLabel");
            int pimAttributeFamilyId = Convert.ToInt32(model.GetValue("PimAttributeFamilyId"));
            var list = (Convert.ToString(values)?.Split(',').ToList());
            string FamilyCode = Convert.ToString(model.GetValue("FamilyCode"));

            PIMFamilyLocaleListModel _locales = new PIMFamilyLocaleListModel();
            if (list?.Count > 0)
            {
                _locales.FamilyLocales = new List<PIMFamilyLocaleModel>();
                if (list.Count > 2)
                {
                    for (int i = 0; i < list.Count; i = i + 2)
                    {
                        _locales.FamilyLocales.Add(new PIMFamilyLocaleModel()
                        {
                            LocaleId = Convert.ToInt32(list[i + 1]),
                            AttributeFamilyName = list[i],
                            PimAttributeFamilyId = pimAttributeFamilyId
                        });
                    }
                    _locales.FamilyCode = FamilyCode;
                }
                else
                {
                    _locales.FamilyLocales.Add(new PIMFamilyLocaleModel()
                    {
                        LocaleId = Convert.ToInt32(list[1]),
                        AttributeFamilyName = list[0],
                        PimAttributeFamilyId = pimAttributeFamilyId
                    });
                    _locales.FamilyCode = FamilyCode;
                }
            }

            return _locales;
        }

        //This method maps PIMAttributeGroupListModel to List<BaseDropDownList>.
        public static List<BaseDropDownList> ToBaseDropDownList(PIMAttributeGroupListModel attributeGroupList)
        {
            List<BaseDropDownList> unassignedAttributeGroups = new List<BaseDropDownList>();
            if (attributeGroupList?.AttributeGroupList?.Count > 0)
            {
                unassignedAttributeGroups = attributeGroupList.AttributeGroupList.Select(x => new BaseDropDownList
                {
                    id = x.PimAttributeGroupId.ToString(),
                    name = x.AttributeGroupName,
                }).ToList();
            }
            return unassignedAttributeGroups;
        }

        public static List<PIMProductAttributeValuesViewModel> GetAttributeControls(List<PIMProductAttributeValuesViewModel> attributeValueList, List<string> distinctAttributeCodes)
        {
            int indexValue = 0;
            List<PIMProductAttributeValuesViewModel> finalAttributeList = new List<PIMProductAttributeValuesViewModel>();
            if (HelperUtility.IsNotNull(attributeValueList) && HelperUtility.IsNotNull(distinctAttributeCodes))
            {
                finalAttributeList = attributeValueList.Where(x => x != null).GroupBy(x => x.AttributeCode + x.PimAttributeFamilyId).Select(g => g.First()).ToList();

                foreach (string item in distinctAttributeCodes)
                {
                    List<PIMProductAttributeValuesViewModel> attributesList = attributeValueList.Where(x => x != null && x.AttributeCode + x.PimAttributeFamilyId == item?.ToString()).ToList();

                    //Appended keys with property name {AttributeCode}[0]_{PimAttributeId}[1]_{PimAttributeDefaultValueId}[2]_{PimAttributeValueId}[3]_{PimAttributeFamilyId}[4].
                    string controlName = $"{attributesList[0].AttributeCode}_{attributesList[0].PimAttributeId}_{attributesList[0].PimAttributeDefaultValueId.GetValueOrDefault()}_{attributesList[0].PimAttributeValueId.GetValueOrDefault()}_{attributesList[0].PimAttributeFamilyId}";

                    finalAttributeList[indexValue].ControlProperty.Id = $"{controlName}_attr";
                    finalAttributeList[indexValue].ControlProperty.ControlType = attributesList[0].AttributeTypeName;
                    finalAttributeList[indexValue].ControlProperty.Name = $"{controlName}_attr";
                    finalAttributeList[indexValue].ControlProperty.ControlLabel = attributesList[0].AttributeName;
                    string attributeDefaultValueCode = GetAttributeDefaultValueCode(attributesList, ZnodeConstant.AttributeDefaultValueCode);
                    attributesList[0].AttributeValue = GetAttributeDefaultValueCode(attributesList, ZnodeConstant.AttributeDefaultValue);
                    finalAttributeList[indexValue].ControlProperty.Value = string.IsNullOrEmpty(attributesList[0].AttributeValue) ? attributesList[0].AttributeDefaultValue : attributesList[0].AttributeValue;
                    finalAttributeList[indexValue].ControlProperty.HelpText = attributesList[0].HelpDescription;
                    finalAttributeList[indexValue].ControlProperty.FilesName = attributesList[0].FilesName;
                    if (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.MultiSelect.ToString()) || Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.SimpleSelect.ToString()))
                    {
                        finalAttributeList[indexValue].ControlProperty.SelectOptions = new List<SelectListItem>();
                        var SelectOptionsList = attributesList.Select(x => new { x.AttributeDefaultValue, x.AttributeDefaultValueCode }).ToList();
                        finalAttributeList[indexValue].ControlProperty.Value = attributeDefaultValueCode; 
                        foreach (var SelectOptions in SelectOptionsList)
                        {
                            if (!string.IsNullOrEmpty(SelectOptions?.AttributeDefaultValueCode))
                            {
                                finalAttributeList[indexValue].ControlProperty.SelectOptions.Add(new SelectListItem() { Text = SelectOptions.AttributeDefaultValue, Value = SelectOptions.AttributeDefaultValueCode });                                
                                finalAttributeList[indexValue].ControlProperty.CSSClass = finalAttributeList[indexValue].AttributeCode;
                            }
                        }
                    }
                    if (attributesList[0].IsRequired)
                    {
                        if (IsKeyNotPresent(AdminConstants.IsRequired, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                            finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(AdminConstants.IsRequired, attributesList[0].IsRequired);
                    }

                    if (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.Label.ToString()))
                        finalAttributeList[indexValue].ControlProperty.Value = attributesList[0].AttributeDefaultValue;

                    foreach (var dataItem in attributesList)
                    {
                        if (!Equals(dataItem.ValidationName, null) && !Equals(dataItem.ValidationName, AdminConstants.Extensions))
                        {
                            if (Equals(dataItem.ControlName, AdminConstants.Select) || Equals(dataItem.ControlName, AdminConstants.MultiSelect) || Equals(dataItem.ControlName, ControlTypes.SimpleSelect.ToString()))
                            {
                                if (IsKeyNotPresent(dataItem.ValidationName, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                                    finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(dataItem.ValidationName, dataItem.SubValidationName);
                            }
                            else
                            {
                                if (IsKeyNotPresent(dataItem.ValidationName, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                                    finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(dataItem.ValidationName, dataItem.ValidationValue);
                            }
                        }
                        else if ((attributesList.Select(x => x.ValidationName == AdminConstants.Extensions).ToList()).Any(m => m))
                        {
                            if (finalAttributeList[indexValue].ControlProperty.htmlAttributes.ContainsKey(AdminConstants.Extensions) == false)
                            {
                                if (IsKeyNotPresent(AdminConstants.Extensions, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                                {
                                    string result = string.Join(",", attributesList.Where(x => x.ValidationName == AdminConstants.Extensions).Select(k => k.SubValidationName).ToArray());
                                    finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(AdminConstants.Extensions, result);
                                }
                            }
                        }
                    }
                    indexValue++;
                }
            }

            return finalAttributeList;
        }
        //This Method will be returns AttributeDefaultValueCode or AttributeDefaultValue or selected attribute value
        private static string GetAttributeDefaultValueCode(List<PIMProductAttributeValuesViewModel> attributeValueList,string attributeValueType)
        {
            return string.IsNullOrEmpty(attributeValueList[0].AttributeValue) ? attributeValueType == ZnodeConstant.AttributeDefaultValueCode ? attributeValueList?.FirstOrDefault(x => x.IsDefault == true)?.AttributeDefaultValueCode : 
                attributeValueList?.FirstOrDefault(x => x.IsDefault == true)?.AttributeDefaultValue : attributeValueList[0].AttributeValue;               
        }
        //This method maps PIMAttributesListModel to List<BaseDropDownList>.
        public static List<BaseDropDownList> ToBaseDropDownAttributeList(PIMAttributeListModel attributeList)
        {
            List<BaseDropDownList> unassignedAttributes = new List<BaseDropDownList>();
            if (attributeList?.Attributes?.Count > 0)
            {
                unassignedAttributes = attributeList.Attributes.Select(x => new BaseDropDownList
                {
                    id = x.PimAttributeId.ToString(),
                    name = x.AttributeName,
                }).ToList();
            }
            return unassignedAttributes;
        }
    }
}