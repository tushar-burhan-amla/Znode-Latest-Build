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
    public static class AttributesModelMap
    {
        #region Public Methods

        //Get Values From BinddataModel into Attributedatamodel
        public static AttributesDataModel ToDataModel(BindDataModel model)
        {
            if (!Equals(model, null))
            {
                return new AttributesDataModel()
                {
                    AttributeTypeId = Convert.ToInt32(model.GetValue("AttributeTypeId")),
                    AttributeGroupId = Equals(Convert.ToString(model.GetValue("AttributeGroupId")), "") ? (int?)null : Convert.ToInt32(model.GetValue("AttributeGroupId")),
                    IsRequired = Convert.ToBoolean(model.GetValue("IsRequired")),
                    MediaAttributeId = Convert.ToInt32(model.GetValue("AttributeId")),
                    IsLocalizable = Convert.ToBoolean(model.GetValue("IsLocalizable")),
                    AttributeCode = model.GetValue("AttributeCode").ToString(),
                    HelpDescription = model.GetValue("HelpDescription").ToString(),
                    DisplayOrder = Equals(Convert.ToString(model.GetValue(ZnodeMediaAttributeEnum.DisplayOrder.ToString())), "")
                                    ? (int?)null : Convert.ToInt32(model.GetValue(ZnodeMediaAttributeEnum.DisplayOrder.ToString())),
                    IsSystemDefined = Equals(Convert.ToString(model.GetValue(ZnodeMediaAttributeEnum.IsSystemDefined.ToString())), "")
                                        ? false : Convert.ToBoolean(model.GetValue(ZnodeMediaAttributeEnum.IsSystemDefined.ToString())),
                };
            }
            return null;
        }

        //Mapping for attbuteinputvalidationlist
        public static List<AttributeInputValidationModel> ToInputValidationListModel(List<AttributeInputValidationDataModel> model)
        {
     
            if (!Equals(model, null))
            {
                string attributeTypeName = model.Select(x => x.AttributeTypeName).FirstOrDefault();

                List<AttributeInputValidationModel> list = (from _item in model
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
                    x.ControlProperty.Values = x.DefaultValue?.Split(',');
                    x.ControlProperty.Value = x.DefaultValue;
                    x.ControlProperty.SelectOptions = GetRules(x.Rules);
                    x.ControlProperty.htmlAttributes = GetValidation(x.ControlName, x.Name, attributeTypeName);
                });
                return list;
            }
            else
                return new List<AttributeInputValidationModel>();
        }


        //Mapping Of LocalelistModel To LocaleDataModel
        public static List<LocaleDataModel> ToLocaleDataModel(LocaleListModel model, AttributesLocaleListModel values)
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

        //Convert Into AttributeDefaultValue Model
        public static AttributesDefaultValueModel ToAttributeDefaultValuesModel(string model, int attributeId, string defaultvaluecode, int defaultvalueId = 0)
        {
            var _list = JsonConvert.DeserializeObject<List<dynamic>>(model);
            //_list.Add(defaultvalueId);
            return new AttributesDefaultValueModel
            {
                AttributeId = attributeId,
                ValueLocales = (from _item in _list
                                select new DefaultAttributeValueLocaleModel
                                {
                                    LocaleId = _item.Id,
                                    DefaultAttributeValue = _item.Value,
                                }).ToList(),
                DefaultAttributeValueId = defaultvalueId,
                AttributeDefaultValueCode= defaultvaluecode

            };
        }

        //Convert Into List Of Default values from locale list model adn attributedefaultvaluelist model
        public static List<DefaultValueListModel> ToDefaultValues(LocaleListModel model, AttributesDefaultValueListModel defaultValues)
        {
            return (from _list in model.Locales
                    select new DefaultValueListModel
                    {
                        LocaleId = _list.LocaleId,
                        LocaleName = _list.Name,
                        LocaleCode = _list.Code,
                        DefaultValues = DefaultValues(_list.LocaleId, defaultValues),
                        AttributeDefaultValueCodeList = ToPIMAttributeDefaultValueListModel(defaultValues)
                    }).ToList();
        }
        public static List<PIMAttributeDefaultValueModel> ToPIMAttributeDefaultValueListModel(AttributesDefaultValueListModel model)
        {
            if (model?.DefaultValues?.Count > 0)
            {
                return (from item in model.DefaultValues
                        select new PIMAttributeDefaultValueModel()
                        {
                            AttributeDefaultValueCode = item.AttributeDefaultValueCode,
                            PimDefaultAttributeValueId = item.DefaultAttributeValueId,
                        }).ToList();
            }
            return new List<PIMAttributeDefaultValueModel>();
        }
        private static List<DefaultValueModel> DefaultValues(int localId, AttributesDefaultValueListModel defaultValues)
        {
            var model = new List<DefaultValueModel>();
            if (!Equals(defaultValues, null))
            {
                defaultValues.DefaultValues.ForEach(defaultValueItem =>
                {
                    DefaultAttributeValueLocaleModel attributeDefaultValueLocaleModel = defaultValueItem?.ValueLocales?.Where(y => y.LocaleId == localId)?.FirstOrDefault();
                    string defaultAttributeValue = attributeDefaultValueLocaleModel?.DefaultAttributeValue;
                    if (string.IsNullOrEmpty(defaultAttributeValue))
                        model.Add(new DefaultValueModel() { LocaleId = localId, Value = null, DefaultAttributeValueId = 0 });
                    else
                        model.Add(new DefaultValueModel() { LocaleId = localId, Value = defaultAttributeValue, DefaultAttributeValueId = attributeDefaultValueLocaleModel?.MediaAttributeDefaultValueId });
                });
            }
            return model;
        }

        public static List<AttributeGroupViewModel> ToAttributeGroupListModel(AttributeGroupListModel model)
        {
            if (model?.AttributeGroups?.Count > 0)
            {
                return (from item in model.AttributeGroups
                        select new AttributeGroupViewModel()
                        {
                            MediaAttributeGroupId = item.MediaAttributeGroupId,
                            GroupCode = item.GroupCode,
                        }).ToList();
            }
            return new List<AttributeGroupViewModel>();
        }

        public static List<AttributesValidationModel> ToAttributeValidationModel(BindDataModel model, List<AttributeInputValidationModel> list)
        {
            // Create Attribute validation list for all attribute validation
            List<AttributesValidationModel> validationList = new List<AttributesValidationModel>();
            list.ForEach(item =>
            {
                object inputValidation = model.GetValue(item.Name.Trim());

                if (!Convert.ToBoolean(item.IsList))
                    validationList.Add(new AttributesValidationModel { InputValidationId = item.AttributeValidationId, Name = Convert.ToString(inputValidation), InputValidationRuleId = null });
                else
                {
                    if (!Equals(inputValidation, null))
                    {
                        //Create string list form comma separated inputvalidationruleIds
                        Convert.ToString(inputValidation).Split(',').ToList().ForEach(inputValidationValueArrayItem =>
                        {
                            //Add one attribute validation item for each inputvalidation rule
                            validationList.Add(new AttributesValidationModel
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

        //*
        //Convert Into AttributeLocaleList model from bind data Model
        public static AttributesLocaleListModel ToLocaleListModel(BindDataModel model)
        {
            var values = model.GetValue("LocaleLabel");
            int AttributeId = Convert.ToInt32(model.GetValue("AttributeId"));
            string attributCode = model.GetValue("AttributeCode").ToString();
            var list = (Convert.ToString(values).Split(',').ToList());

            AttributesLocaleListModel _locales = new AttributesLocaleListModel();

            if (list.Count > 2)
            {
                for (int i = 0; i < list.Count; i = i + 2)
                {
                    _locales.Locales.Add(new AttributesLocaleModel()
                    {
                        LocaleId = Convert.ToInt32(list[i + 1]),
                        AttributeName = list[i],
                        MediaAttributeId = AttributeId
                    });
                }
            }
            else
            {
                _locales.Locales.Add(new AttributesLocaleModel()
                {
                    LocaleId = Convert.ToInt32(list[1]),
                    AttributeName = list[0],
                    MediaAttributeId = AttributeId
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
                        select new SelectListItem
                        {
                            Text = _list.ValidationName,
                            Value = _list.InputValidationRuleId.ToString()
                        }).ToList();
            }
            return null;
        }

        //Mapping For Validation Rule List
        public static List<SelectListItem> GetAttributeGroups(int attributeGroupId, IEnumerable<AttributeGroupModel> model)
        {
            if (!Equals(model, null))
            {
                return (from _list in model
                        select new SelectListItem
                        {
                            Text = _list.GroupCode,
                            Value = _list.MediaAttributeGroupId.ToString(),
                            Selected = attributeGroupId == _list.MediaAttributeGroupId
                        }).ToList();
            }
            return null;
        }
        #endregion
    }
}

