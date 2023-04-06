using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Engine.Admin.Helpers.HelperMethods;

namespace Znode.Engine.Admin
{
    public static class PIMAttributeViewModelMap
    {
        #region Public Methods

        //Convert PIMAttributeListModel to PIMAttributeListViewModel.
        public static PIMAttributeListViewModel ToListViewModel(PIMAttributeListModel attributeListModel)
        {
            PIMAttributeListViewModel listViewModel = new PIMAttributeListViewModel();
            if (!Equals(attributeListModel, null))
            {
                listViewModel.List = new List<PIMAttributeViewModel>();
                if (!Equals(attributeListModel.Attributes, null) && attributeListModel.Attributes.Count > 0)
                {
                    foreach (PIMAttributeModel model in attributeListModel.Attributes)
                    {
                        //Maps the API attributes list model with  attributes list model.
                        listViewModel.List.Add(ToPIMAttributeViewModel(model));
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

        //Mapping of AttributeTypeModel to AttributeTypeList.
        public static List<AttributeTypeModel> ToAttributeTypeListModel(List<PIMAttributeTypeModel> model)
        {
            return (from item in model
                    orderby item.AttributeTypeName ascending
                    select new AttributeTypeModel
                    {
                        AttributeTypeId = item.AttributeTypeId,
                        AttributeTypeName = item.AttributeTypeName
                    }).ToList();
        }

        //Mapping of Attribute Data Model to AttributeDataViewModel.
        public static PIMAttributeDataViewModel ToDataViewModel(PIMAttributeDataModel dataModel)
        {
            if (!Equals(dataModel, null))
            {
                return new PIMAttributeDataViewModel()
                {
                    AttributeViewModel = ToPIMAttributeViewModel(dataModel.AttributeModel),
                    FrontProperties = ToFrontViewModel(dataModel.FrontProperties),
                };
            }
            return null;
        }

        //Convert PIMAttributeModel to PIMAttributeViewModel.
        public static PIMAttributeViewModel ToPIMAttributeViewModel(PIMAttributeModel attributeModel)
        {
            if (!Equals(attributeModel, null))
            {
                return new PIMAttributeViewModel()
                {
                    AttributeTypeId = attributeModel.AttributeTypeId,
                    AttributeGroupId = attributeModel.AttributeGroupId,
                    PimAttributeId = attributeModel.PimAttributeId,
                    IsRequired = attributeModel.IsRequired,
                    AttributeName = attributeModel.AttributeName,
                    AttributeTypeName = attributeModel.AttributeTypeName,
                    IsConfigurable = attributeModel.IsConfigurable,
                    IsPersonalizable = attributeModel.IsPersonalizable,
                    AttributeCode = attributeModel.AttributeCode,
                    AttributeDisplayOrder = attributeModel.AttributeDisplayOrder,
                    HelpDescription = attributeModel.HelpDescription,
                    DisplayOrder = attributeModel.DisplayOrder,
                    IsSystemDefined = attributeModel.IsSystemDefined,
                    IsLocalizable = attributeModel.IsLocalizable,
                    AttributeDefaultValue = attributeModel.AttributeDefaultValue,
                    AttributeDefaultValueId = attributeModel.AttributeDefaultValueId,
                    IsShowOnGrid = attributeModel.IsShowOnGrid,
                    IsSwatchImage = attributeModel.IsSwatch,
                    IsComparable=attributeModel.IsComparable,
                    IsFacets=attributeModel.IsFacets,
                    IsUseInSearch=attributeModel.IsUseInSearch,
                    UsedInProductsCount = attributeModel.UsedInProductsCount
                };
            }
            return null;
        }

        //Get values from BindData Model into AttributeDataModel.
        public static PIMAttributeDataModel ToDataModel(BindDataModel model)
        {
            int attributeTypeId = Convert.ToInt32(model.GetValue("AttributeTypeId"));

            if (!Equals(attributeTypeId, null))
            {
                return new PIMAttributeDataModel()
                {
                    AttributeModel = ToPIMAttributeModel(model),
                    FrontProperties = ToFrontModel(model)

                };
            }
            return null;
        }

        //Mapping for attbuteinputvalidationlist.
        public static List<AttributeInputValidationModel> ToInputValidationListModel(List<PIMAttributeInputValidationModel> model)
        {
            if (!Equals(model, null))
            {
                string attributeTypeName = model.Select(x => x.AttributeTypeName).FirstOrDefault();

                var list = (from _item in model
                            select new AttributeInputValidationModel
                            {
                                AttributeTypeId = _item.AttributeTypeId,
                                AttributeValidationId = _item.AttributeValidationId,
                                ControlName = _item.ControlName,
                                DefaultValue = _item.DefaultValue,
                                DisplayOrder = _item.DisplayOrder,
                                IsList = _item.IsList,
                                ValidationName = _item.ValidationName,
                                Name = _item.Name,
                                Rules = (from _rules in _item.Rules
                                         select new AttributeValidationRuleModel
                                         {
                                             DisplayOrder = _rules.DisplayOrder,
                                             InputValidationId = _rules.InputValidationId,
                                             InputValidationRuleId = _rules.InputValidationRuleId,
                                             RegExp = _rules.RegExp,
                                             ValidationName = _rules.ValidationName,
                                             ValidationRule = _rules.ValidationRule
                                         }).ToList()
                            }).ToList();
                list.ForEach(x =>
                {
                    x.ControlProperty = new Property();
                    x.ControlProperty.Name = x.Name;
                    x.ControlProperty.ControlType = x.ControlName;
                    x.ControlProperty.Id = x.Name;
                    if ((x.DefaultValue == "0" || x.DefaultValue == null || x.DefaultValue == "") & (x.ControlProperty.Id == "MaxFileSize") & (x.ControlProperty.Value == "0" || x.ControlProperty.Value == null))
                    {
                        x.DefaultValue = "10";
                        x.ControlProperty.Value = x.DefaultValue;
                    }
                    else
                    {
                        x.ControlProperty.Value = x.DefaultValue;
                    }
                    x.ControlProperty.Values = x.DefaultValue?.Split(',');
                    x.ControlProperty.SelectOptions = GetRules(x.Rules);
                    x.ControlProperty.htmlAttributes = GetValidation(x.ControlName, x.Name, attributeTypeName);
                });
                return list;
            }
            else
                return new List<AttributeInputValidationModel>();
        }

        //Convert FrontPropertiesModel To FrontPropertiesViewModel
        public static PIMFrontPropertiesViewModel ToFrontEndViewModel(PIMFrontPropertiesModel model)
        {
            PIMFrontPropertiesViewModel viewModel = new PIMFrontPropertiesViewModel();
            if (!Equals(model, null))
            {
                viewModel.IsComparable = model.IsComparable;
                viewModel.IsUseInSearch = model.IsUseInSearch;
                viewModel.IsAllowHtmlTag = model.IsAllowHtmlTag;
                viewModel.IsFacets = model.IsFacets;
            }
            return viewModel;
        }

        //Convert into PIMAttributeModel from Bind Data Model
        public static PIMAttributeModel ToAttributeModel(BindDataModel model)
        {
            return new PIMAttributeModel()
            {
                AttributeTypeId = Convert.ToInt32(model.GetValue("AttributeTypeId"))
            };
        }

        //Mapping Of LocalelistModel To LocaleDataModel
        public static List<LocaleDataModel> ToLocaleDataModel(LocaleListModel model, PIMAttributeLocaleListModel values)
        {
            return (from _list in model.Locales
                    select new LocaleDataModel
                    {
                        LocaleId = _list.LocaleId,
                        Name = _list.Name,
                        Code = _list.Code,
                        IsActive = _list.IsActive,
                        IsDefault = _list.IsDefault,
                        Value = Equals(values, null) ? null : !Equals(values.Locales.FirstOrDefault(x => x.LocaleId == _list.LocaleId), null) ? values.Locales.FirstOrDefault(x => x.LocaleId == _list.LocaleId).AttributeName : null
                    }).ToList();
        }

        //Convert Into AttributeDefaultValue Model.
        public static PIMAttributeDefaultValueModel ToAttributeDefaultValuesModel(string model, int attributeId, string defaultvaluecode, bool isDefault, bool isswatch, string swatchtext, int displayOrder = 0, int defaultvalueId = 0)
        {
            var _list = JsonConvert.DeserializeObject<List<dynamic>>(model);
            return new PIMAttributeDefaultValueModel
            {
                PimAttributeId = attributeId,
                ValueLocales = (from _item in _list
                                select new PIMAttributeDefaultValueLocaleModel
                                {
                                    LocaleId = _item.Id,
                                    DefaultAttributeValue = _item.Value,
                                }).ToList(),
                PimDefaultAttributeValueId = defaultvalueId,
                AttributeDefaultValueCode = defaultvaluecode,
                DisplayOrder = displayOrder,
                IsDefault = isDefault,
                IsSwatch = isswatch,
                SwatchText = swatchtext
            };
        }
        //Convert Into List Of Default values from locale list model adn attributedefaultvaluelist model
        public static List<DefaultValueListModel> ToDefaultValues(LocaleListModel model, PIMAttributeDefaultValueListModel defaultValues)
        {

            return (from _list in model.Locales
                    select new DefaultValueListModel
                    {
                        LocaleId = _list.LocaleId,
                        LocaleName = _list.Name,
                        LocaleCode = _list.Code,
                        DefaultValues = DefaultValues(_list.LocaleId, defaultValues),
                        AttributeDefaultValueCodeList = defaultValues.DefaultValues
                    }).ToList();
        }

        //Get list of DefaultValueModel as per to locale.
        private static List<DefaultValueModel> DefaultValues(int localId, PIMAttributeDefaultValueListModel defaultValues)
        {
            var model = new List<DefaultValueModel>();
            if (!Equals(defaultValues, null))
            {
                defaultValues.DefaultValues.ForEach(defaultValueItem =>
                {
                    PIMAttributeDefaultValueLocaleModel pimAttributeDefaultValueLocaleModel = defaultValueItem?.ValueLocales?.Where(y => y.LocaleId == localId)?.FirstOrDefault();
                    string defaultAttributeValue = pimAttributeDefaultValueLocaleModel?.DefaultAttributeValue;
                    if (string.IsNullOrEmpty(defaultAttributeValue))
                        model.Add(new DefaultValueModel() { LocaleId = localId, Value = null, DefaultAttributeValueId = 0 });
                    else
                        model.Add(new DefaultValueModel() { LocaleId = localId, Value = defaultAttributeValue, DefaultAttributeValueId = pimAttributeDefaultValueLocaleModel?.PimDefaultAttributeValueId });
                });
            }
            return model;
        }

        //Get list of validations.
        public static List<PIMAttributeValidationModel> ToAttributeValidationModel(BindDataModel model, List<AttributeInputValidationModel> list)
        {
            // Create Attribute validation list for all attribute validation
            List<PIMAttributeValidationModel> validationList = new List<PIMAttributeValidationModel>();
            list.ForEach(item =>
            {
                object inputValidation = model.GetValue(item.Name.Trim());

                if (!Convert.ToBoolean(item.IsList))
                    validationList.Add(new PIMAttributeValidationModel { InputValidationId = item.AttributeValidationId, Name = Convert.ToString(inputValidation), InputValidationRuleId = null });
                else
                {
                    if (!Equals(inputValidation, null))
                    {
                        //Create string list form comma separated inputvalidationruleIds
                        Convert.ToString(inputValidation).Split(',').ToList().ForEach(inputValidationValueArrayItem =>
                        {
                            //Add one attribute validation item for each inputvalidation rule
                            validationList.Add(new PIMAttributeValidationModel
                            {
                                InputValidationId = item.AttributeValidationId,
                                Name = null,
                                InputValidationRuleId = inputValidationValueArrayItem.GetIntegerValue()
                            });
                        });
                    }
                }
            });
            return validationList;
        }

        //Convert Into AttributeLocaleList model from bind data Model
        public static PIMAttributeLocaleListModel ToLocaleListModel(BindDataModel model)
        {
            var values = model.GetValue("LocaleLabel");
            int pimAttributeId = Convert.ToInt32(model.GetValue("AttributeId"));
            string attributCode = model.GetValue("AttributeCode").ToString();
            var list = (Convert.ToString(values).Split(',').ToList());

            PIMAttributeLocaleListModel _locales = new PIMAttributeLocaleListModel();

            if (list.Count > 2)
            {
                for (int i = 0; i < list.Count; i = i + 2)
                {
                    _locales.Locales.Add(new PIMAttributeLocaleModel()
                    {
                        LocaleId = Convert.ToInt32(list[i + 1]),
                        AttributeName = list[i],
                        PimAttributeId = pimAttributeId
                    });
                }
            }
            else
            {
                _locales.Locales.Add(new PIMAttributeLocaleModel()
                {
                    LocaleId = Convert.ToInt32(list[1]),
                    AttributeName = list[0],
                    PimAttributeId = pimAttributeId
                });
            }

            _locales.AttributeCode = attributCode;
            return _locales;
        }

        #endregion

        #region Private Methods
        //Mapping For Validation Rule List
        private static List<SelectListItem> GetRules(List<AttributeValidationRuleModel> Rules)
        {
            if (!Equals(Rules, null))
            {
                return (from _list in Rules
                        orderby _list.ValidationName ascending
                        select new SelectListItem
                        {
                            Text = _list.ValidationName,
                            Value = _list.InputValidationRuleId.ToString()
                        }).ToList();
            }
            return null;
        }


        //Mapping Of Values To AttributeModel from BindDataModel
        private static PIMAttributeModel ToPIMAttributeModel(BindDataModel model)
        {

            if (!Equals(model, null))
            {
                bool? isSwatch = null;
                if (Convert.ToBoolean(model.GetValue(ZnodePimAttributeEnum.IsCategory.ToString())))
                {
                    isSwatch = (bool?)null;
                }
                else
                {
                    if (model.GetValue(ZnodeConstant.IsSwatchImage).ToString() == "" || model.GetValue(ZnodeConstant.IsSwatchImage).ToString() == "null" || model.GetValue(ZnodeConstant.IsSwatchImage) == null)                 
                        isSwatch = (bool?)null;
                    else                   
                        isSwatch = Convert.ToBoolean(model.GetValue(ZnodeConstant.IsSwatchImage.ToString()));
                }
                return new PIMAttributeModel()
                {
                    AttributeTypeId = Convert.ToInt32(model.GetValue(ZnodePimAttributeEnum.AttributeTypeId.ToString())),
                    AttributeGroupId = Equals(Convert.ToString(model.GetValue("PimAttributeGroupId")), "")
                                        ? (int?)null : Convert.ToInt32(model.GetValue("PimAttributeGroupId")),
                    IsRequired = Convert.ToBoolean(model.GetValue(ZnodePimAttributeEnum.IsRequired.ToString())),
                    PimAttributeId = Convert.ToInt32(model.GetValue("AttributeId")),
                    IsConfigurable = Convert.ToBoolean(model.GetValue(ZnodePimAttributeEnum.IsConfigurable.ToString())),
                    IsPersonalizable = Convert.ToBoolean(model.GetValue(ZnodePimAttributeEnum.IsPersonalizable.ToString())),
                    AttributeCode = model.GetValue(ZnodePimAttributeEnum.AttributeCode.ToString()).ToString(),
                    HelpDescription = Equals(Convert.ToString(model.GetValue("HelpDescription")), "") ? null : Convert.ToString(model.GetValue("HelpDescription")),
                    DisplayOrder = Equals(Convert.ToString(model.GetValue(ZnodePimAttributeEnum.DisplayOrder.ToString())), "")
                                    ? (int?)null : Convert.ToInt32(model.GetValue(ZnodePimAttributeEnum.DisplayOrder.ToString())),
                    IsSystemDefined = Equals(Convert.ToString(model.GetValue(ZnodePimAttributeEnum.IsSystemDefined.ToString())), "")
                                        ? false : Convert.ToBoolean(model.GetValue(ZnodePimAttributeEnum.IsSystemDefined.ToString())),
                    IsCategory = Convert.ToBoolean(model.GetValue(ZnodePimAttributeEnum.IsCategory.ToString())),
                    IsLocalizable = Convert.ToBoolean(model.GetValue(ZnodePimAttributeEnum.IsLocalizable.ToString())),
                    AttributeDefaultValue = Convert.ToString(model.GetValue("AttributeDefaultValue")),
                    AttributeDefaultValueId = Equals(Convert.ToString(model.GetValue("AttributeDefaultValueId")), "")
                                        ? (int?)null : Convert.ToInt32(model.GetValue("AttributeDefaultValueId")),
                    IsHidden = false,
                    IsShowOnGrid = Equals(Convert.ToString(model.GetValue(ZnodePimAttributeEnum.IsShowOnGrid.ToString())), "")
                                        ? false : Convert.ToBoolean(model.GetValue(ZnodePimAttributeEnum.IsShowOnGrid.ToString())),

                    IsSwatch = isSwatch,
                };
            }
            return null;
        }

        //Mapping Of Values To FrontPropertiesModel  from BindDataModel
        private static PIMFrontPropertiesModel ToFrontModel(BindDataModel model)
        {
            if (!Equals(model, null))
            {
                return new PIMFrontPropertiesModel()
                {
                    IsAllowHtmlTag = Convert.ToBoolean(model.GetValue("IsAllowHtmlTag")),
                    IsComparable = Convert.ToBoolean(model.GetValue("IsComparable")),
                    IsUseInSearch = Convert.ToBoolean(model.GetValue("IsUseInSearch")),
                    IsFacets = Convert.ToBoolean(model.GetValue("IsFacets"))
                };
            }
            return null;
        }

        //Convert FrontPropertiesModel to FrontPropertiesViewMOdel
        private static PIMFrontPropertiesViewModel ToFrontViewModel(PIMFrontPropertiesModel frontProperties)
        {
            if (!Equals(frontProperties, null))
            {
                return new PIMFrontPropertiesViewModel()
                {
                    IsAllowHtmlTag = frontProperties.IsAllowHtmlTag,
                    IsUseInSearch = frontProperties.IsUseInSearch,
                    IsComparable = frontProperties.IsComparable,
                };
            }
            return new PIMFrontPropertiesViewModel();
        }

        //Mapping For Validation Rule List
        public static List<SelectListItem> GetAttributeGroups(int attributeGroupId, IEnumerable<PIMAttributeGroupModel> model)
        {
            if (!Equals(model, null))
            {
                return (from _list in model
                        orderby _list.GroupCode ascending
                        select new SelectListItem
                        {
                            Text = _list.GroupCode,
                            Value = _list.PimAttributeGroupId.ToString(),
                            Selected = attributeGroupId == _list.PimAttributeGroupId
                        }).ToList();
            }
            return null;
        }
        #endregion
    }
}

