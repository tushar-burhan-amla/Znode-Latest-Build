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
    public static class GlobalAttributeViewModelMap
    {
        #region Public Methods
        //Convert GlobalAttributeModel to GlobalAttributeViewModel.
        public static GlobalAttributeViewModel ToGlobalAttributeViewModel(GlobalAttributeModel attributeModel)
        {
            if (!Equals(attributeModel, null))
            {
                return new GlobalAttributeViewModel()
                {
                    AttributeTypeId = attributeModel.AttributeTypeId,
                    GlobalAttributeId = attributeModel.GlobalAttributeId,
                    IsRequired = attributeModel.IsRequired,
                    AttributeName = attributeModel.AttributeName,
                    AttributeTypeName = attributeModel.AttributeTypeName,
                    AttributeCode = attributeModel.AttributeCode,
                    AttributeDisplayOrder = attributeModel.AttributeDisplayOrder,
                    HelpDescription = attributeModel.HelpDescription,
                    DisplayOrder = attributeModel.DisplayOrder,
                    IsLocalizable = attributeModel.IsLocalizable,
                    AttributeDefaultValue = attributeModel.AttributeDefaultValue,
                    AttributeDefaultValueId = attributeModel.AttributeDefaultValueId
                };
            }
            return null;
        }

        //Mapping Of Values To AttributeModel from BindDataModel
        public static GlobalAttributeModel ToGlobalAttributeModel(BindDataModel model)
        {
            if (!Equals(model, null))
            {
                return new GlobalAttributeModel()
                {
                    AttributeTypeId = Convert.ToInt32(model.GetValue(ZnodeGlobalAttributeEnum.AttributeTypeId.ToString())),
                    IsRequired = Convert.ToBoolean(model.GetValue(ZnodeGlobalAttributeEnum.IsRequired.ToString())),
                    GlobalAttributeId = Convert.ToInt32(model.GetValue("AttributeId")),
                    AttributeCode = model.GetValue(ZnodeGlobalAttributeEnum.AttributeCode.ToString()).ToString(),
                    HelpDescription = Equals(Convert.ToString(model.GetValue("HelpDescription")), "") ? null : Convert.ToString(model.GetValue("HelpDescription")),
                    DisplayOrder = Equals(Convert.ToString(model.GetValue(ZnodeGlobalAttributeEnum.DisplayOrder.ToString())), "")
                                    ? (int?)null : Convert.ToInt32(model.GetValue(ZnodeGlobalAttributeEnum.DisplayOrder.ToString())),
                    IsLocalizable = Convert.ToBoolean(model.GetValue(ZnodeGlobalAttributeEnum.IsLocalizable.ToString())),
                    AttributeDefaultValue = Convert.ToString(model.GetValue("AttributeDefaultValue")),
                    AttributeDefaultValueId = Equals(Convert.ToString(model.GetValue("AttributeDefaultValueId")), "")
                                        ? (int?)null : Convert.ToInt32(model.GetValue("AttributeDefaultValueId")),
                    GlobalEntityId = Convert.ToInt32(model.GetValue("GlobalEntityId"))
                };
            }
            return null;
        }

        //Get list of validations.
        public static List<GlobalAttributeValidationModel> ToAttributeValidationModel(BindDataModel model, List<AttributeInputValidationModel> list)
        {
            // Create Attribute validation list for all attribute validation
            List<GlobalAttributeValidationModel> validationList = new List<GlobalAttributeValidationModel>();
            list.ForEach(item =>
            {
                object inputValidation = model.GetValue(item.Name.Trim());

                if (!Convert.ToBoolean(item.IsList))
                    validationList.Add(new GlobalAttributeValidationModel { InputValidationId = item.AttributeValidationId, Name = Convert.ToString(inputValidation), InputValidationRuleId = null });
                else
                {
                    if (!Equals(inputValidation, null))
                    {
                        //Create string list form comma separated inputvalidationruleIds
                        Convert.ToString(inputValidation).Split(',').ToList().ForEach(inputValidationValueArrayItem =>
                        {
                            //Add one attribute validation item for each inputvalidation rule
                            validationList.Add(new GlobalAttributeValidationModel
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
        public static GlobalAttributeLocaleListModel ToLocaleListModel(BindDataModel model)
        {
            var values = model.GetValue("LocaleLabel");
            int globalAttributeId = Convert.ToInt32(model.GetValue("AttributeId"));
            string attributCode = model.GetValue("AttributeCode").ToString();
            var list = (Convert.ToString(values).Split(',').ToList());

            GlobalAttributeLocaleListModel _locales = new GlobalAttributeLocaleListModel();

            if (list.Count > 2)
            {
                for (int i = 0; i < list.Count; i = i + 2)
                {
                    _locales.Locales.Add(new GlobalAttributeLocaleModel()
                    {
                        LocaleId = Convert.ToInt32(list[i + 1]),
                        AttributeName = list[i],
                        GlobalAttributeId = globalAttributeId
                    });
                }
            }
            else
            {
                _locales.Locales.Add(new GlobalAttributeLocaleModel()
                {
                    LocaleId = Convert.ToInt32(list[1]),
                    AttributeName = list[0],
                    GlobalAttributeId = globalAttributeId
                });
            }

            _locales.AttributeCode = attributCode;
            return _locales;
        }

        //Mapping for attbuteinputvalidationlist.
        public static List<AttributeInputValidationModel> ToInputValidationListModel(List<GlobalAttributeInputValidationModel> model)
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
                    x.ControlProperty.Value = x.DefaultValue;
                    x.ControlProperty.Values = x.DefaultValue?.Split(',');
                    x.ControlProperty.SelectOptions = GetRules(x.Rules);
                    x.ControlProperty.htmlAttributes = GetValidation(x.ControlName, x.Name, attributeTypeName);
                });
                return list;
            }
            else
                return new List<AttributeInputValidationModel>();
        }

        //Mapping Of LocalelistModel To LocaleDataModel
        public static List<LocaleDataModel> ToLocaleDataModel(LocaleListModel model, GlobalAttributeLocaleListModel values)
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

        //Convert Into List Of Default values from locale list model adn attributedefaultvaluelist model
        public static List<DefaultValueListModel> ToDefaultValues(LocaleListModel model, GlobalAttributeDefaultValueListModel defaultValues)
        {
            return (from _list in model.Locales
                    select new DefaultValueListModel
                    {
                        LocaleId = _list.LocaleId,
                        LocaleName = _list.Name,
                        LocaleCode = _list.Code,
                        DefaultValues = DefaultValues(_list.LocaleId, defaultValues),
                        GlobalAttributeDefaultValueCodeList = defaultValues.DefaultValues
                    }).ToList();
        }

        //Convert Into AttributeDefaultValue Model.
        public static GlobalAttributeDefaultValueModel ToAttributeDefaultValuesModel(string model, int attributeId, string defaultvaluecode, bool isDefault, bool isswatch, string swatchtext, int displayOrder = 0, int defaultvalueId = 0)
        {
            var _list = JsonConvert.DeserializeObject<List<dynamic>>(model);
            return new GlobalAttributeDefaultValueModel
            {
                GlobalAttributeId = attributeId,
                ValueLocales = (from _item in _list
                                select new GlobalAttributeDefaultValueLocaleModel
                                {
                                    LocaleId = _item.Id,
                                    DefaultAttributeValue = _item.Value,
                                }).ToList(),
                GlobalAttributeDefaultValueId = defaultvalueId,
                AttributeDefaultValueCode = defaultvaluecode,
                DisplayOrder = displayOrder,
                IsDefault = isDefault,
                IsSwatch = isswatch,
                SwatchText = swatchtext
            };
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

        //Get list of DefaultValueModel as per to locale.
        private static List<DefaultValueModel> DefaultValues(int localId, GlobalAttributeDefaultValueListModel defaultValues)
        {
            var model = new List<DefaultValueModel>();
            if (!Equals(defaultValues, null))
            {
                defaultValues.DefaultValues.ForEach(defaultValueItem =>
                {
                    GlobalAttributeDefaultValueLocaleModel globalAttributeDefaultValueLocaleModel = defaultValueItem?.ValueLocales?.Where(y => y.LocaleId == localId)?.FirstOrDefault();
                    string defaultAttributeValue = globalAttributeDefaultValueLocaleModel?.DefaultAttributeValue;
                    if (string.IsNullOrEmpty(defaultAttributeValue))
                        model.Add(new DefaultValueModel() { LocaleId = localId, Value = null, DefaultAttributeValueId = 0 });
                    else
                        model.Add(new DefaultValueModel() { LocaleId = localId, Value = defaultAttributeValue, DefaultAttributeValueId = globalAttributeDefaultValueLocaleModel?.GlobalDefaultAttributeValueId });
                });
            }
            return model;
        }
        #endregion
    }
}
