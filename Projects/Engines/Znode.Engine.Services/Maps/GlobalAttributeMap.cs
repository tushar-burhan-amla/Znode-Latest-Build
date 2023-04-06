using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services
{
    public static class GlobalAttributeMap
    {
        #region Public Methods

        //Map attribute entity model to attribute data model
        public static GlobalAttributeDataModel ToGlobalAttributeDataModel(ZnodeGlobalAttribute entity)
        {
            return new GlobalAttributeDataModel()
            {
                AttributeModel = ToAttributeModel(entity)
            };
        }

        //Map GlobalAttribute entity model to AttributeModel
        public static GlobalAttributeModel ToAttributeModel(ZnodeGlobalAttribute entity)
        {
            if (!Equals(entity, null))
            {
                return new GlobalAttributeModel
                {
                    AttributeTypeId = entity.AttributeTypeId,
                    AttributeCode = entity.AttributeCode,
                    GlobalAttributeId = entity.GlobalAttributeId,
                    IsLocalizable = entity.IsLocalizable,
                    IsRequired = entity.IsRequired,
                    HelpDescription = entity.HelpDescription,
                    DisplayOrder = entity.DisplayOrder,
                    Locales = (from _locale in entity.ZnodeGlobalAttributeLocales
                               select new GlobalAttributeLocaleModel
                               {
                                   LocaleId = _locale.LocaleId,
                                   AttributeName = _locale.AttributeName,
                                   GlobalAttributeLocaleId = _locale.GlobalAttributeLocaleId

                               }).ToList()
                };
            }
            else
                return null;
        }

        //Map GlobalAttributeValidationModel to GlobalAttributeValidation entity model
        public static ZnodeGlobalAttributeValidation ToValidationEntity(GlobalAttributeValidationModel model, string attributeType)
        {
            if (!Equals(model, null))
            {
                return new ZnodeGlobalAttributeValidation
                {
                    InputValidationRuleId = model.InputValidationRuleId,
                    InputValidationId = model.InputValidationId,
                    Name = attributeType.Equals(ZnodeConstant.DateType) ? HelperUtility.ConvertStringToSqlDateFormat(model.Name) : model.Name,
                    GlobalAttributeId = model.GlobalAttributeId,
                    GlobalAttributeValidationId = model.GlobalAttributeValidationId
                };
            }
            else return null;
        }

        //Map GlobalAttributeValidationModel to GlobalAttributeValidation entity model
        public static ZnodeGlobalAttributeDefaultValue ToDefaultValueEntity(GlobalAttributeDefaultValueModel model)
        {
            if (!Equals(model, null))
            {
                return new ZnodeGlobalAttributeDefaultValue()
                {
                    GlobalAttributeId = model.GlobalAttributeId,
                    AttributeDefaultValueCode = model.AttributeDefaultValueCode,
                    DisplayOrder = model.DisplayOrder,
                    IsDefault = model.IsDefault,
                    MediaId = Equals(model.IsSwatch, true) ? Convert.ToInt32(model.SwatchText) : model.MediaId,
                    SwatchText = !Equals(model.IsSwatch, true) ? model.SwatchText : string.Empty,
                };
            }
            return null;
        }

        //Map ToGlobalAttributeDefaultValueModel to ToGlobalAttributeDefaultValueModel entity model
        public static GlobalAttributeDefaultValueModel ToGlobalAttributeDefaultValueModel(GlobalAttributeDataModel model, int defaultLocaleId, string attributeType)
        {
            return new GlobalAttributeDefaultValueModel()
            {
                GlobalAttributeDefaultValueId = model.AttributeModel.AttributeDefaultValueId.GetValueOrDefault(),
                ValueLocales = ToGlobalAttributeDefaultValueLocaleModelList(model, defaultLocaleId, attributeType),
                GlobalAttributeId = model.AttributeModel.GlobalAttributeId
            };
        }

        //Map ToGlobalAttributeDefaultValueLocaleModelList to ToGlobalAttributeDefaultValueLocaleModelList entity model
        public static List<GlobalAttributeDefaultValueLocaleModel> ToGlobalAttributeDefaultValueLocaleModelList(GlobalAttributeDataModel model, int defaultLocaleId, string attributeType)
        {
            List<GlobalAttributeDefaultValueLocaleModel> modelList = new List<GlobalAttributeDefaultValueLocaleModel>();
            modelList.Add(new GlobalAttributeDefaultValueLocaleModel()
            {
                GlobalDefaultAttributeValueId = model.AttributeModel.AttributeDefaultValueId.GetValueOrDefault(),
                DefaultAttributeValue = attributeType.Equals(ZnodeConstant.DateType) ? HelperUtility.ConvertStringToSqlDateFormat(model.AttributeModel.AttributeDefaultValue) : model.AttributeModel.AttributeDefaultValue,
                LocaleId = defaultLocaleId
            });
            return modelList;
        }

        //Convert Into Global attribute default value model 
        public static ZnodeGlobalAttributeDefaultValueLocale ToDefaultValueLocaleEntity(GlobalAttributeDefaultValueLocaleModel model)
        {
            if (!Equals(model, null))
            {
                return new ZnodeGlobalAttributeDefaultValueLocale()
                {
                    LocaleId = model.LocaleId,
                    AttributeDefaultValue = model.DefaultAttributeValue,
                    GlobalAttributeDefaultValueId = model.GlobalDefaultAttributeValueId

                };
            }
            return null;
        }

        //Convert Into Global attributeInputValidation list model 
        public static GlobalAttributeInputValidationListModel ToInputValidationListModel(IList<ZnodeAttributeInputValidation> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new GlobalAttributeInputValidationListModel();
                model.InputValidations = new List<GlobalAttributeInputValidationModel>();

                model.InputValidations = (from _list in entity
                                          select new GlobalAttributeInputValidationModel()
                                          {
                                              AttributeTypeId = _list.AttributeTypeId,
                                              AttributeValidationId = _list.InputValidationId,
                                              AttributeTypeName = _list.ZnodeAttributeType.AttributeTypeName,
                                              ControlName = _list.ControlName,
                                              DefaultValue = _list.DefaultValue,
                                              DisplayOrder = _list.DisplayOrder,
                                              IsList = _list.IsList,
                                              Name = _list.Name,
                                              Rules = (from _rules in _list.ZnodeAttributeInputValidationRules
                                                       select new GlobalAttributeValidationRuleModel
                                                       {
                                                           InputValidationId = _rules.InputValidationId,
                                                           InputValidationRuleId = _rules.InputValidationRuleId,
                                                           DisplayOrder = _rules.DisplayOrder,
                                                           RegExp = _rules.RegExp,
                                                           ValidationName = _rules.ValidationName,
                                                           ValidationRule = _rules.ValidationRule
                                                       }).ToList()

                                          }).ToList();
                return model;
            }
            else
                return null;
        }

        public static GlobalAttributeDefaultValueListModel ToDefaultValueListModel(IList<ZnodeGlobalAttributeDefaultValue> entity)
        {
            if (!Equals(entity, null))
            {
                GlobalAttributeDefaultValueListModel model = new GlobalAttributeDefaultValueListModel();
                model.DefaultValues = (from _item in entity
                                       select new GlobalAttributeDefaultValueModel
                                       {
                                           GlobalAttributeId = _item.GlobalAttributeId,
                                           GlobalAttributeDefaultValueId = _item.GlobalAttributeDefaultValueId,
                                           AttributeDefaultValueCode = _item.AttributeDefaultValueCode,
                                           DisplayOrder = _item.DisplayOrder,
                                           IsDefault = _item.IsDefault,
                                           MediaId = _item.MediaId ?? 0,
                                           SwatchText = _item.SwatchText,
                                           ValueLocales = (from _locales in _item.ZnodeGlobalAttributeDefaultValueLocales
                                                           select new GlobalAttributeDefaultValueLocaleModel
                                                           {
                                                               DefaultAttributeValue = _locales.AttributeDefaultValue,
                                                               LocaleId = _locales.LocaleId,
                                                               GlobalAttributeDefaultValueLocaleId = _locales.GlobalAttributeDefaultValueLocaleId,
                                                               GlobalDefaultAttributeValueId = _locales.GlobalAttributeDefaultValueId
                                                           }).ToList()
                                       }).ToList();
                return model;
            }
            return null;
        }

        //Map GlobalAttribute Entity Model to AttributeListModel
        public static GlobalAttributeListModel ToListModel(IList<ZnodeGlobalAttribute> entity)
        {
            if (!Equals(entity, null))
            {
                GlobalAttributeListModel model = new GlobalAttributeListModel();
                foreach (var item in entity)
                {
                    model.Attributes.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }

        //Map Attribute Entity Model to AttributeModel 
        public static GlobalAttributeModel ToModel(ZnodeGlobalAttribute entity)
        {
            if (IsNotNull(entity))
            {
                return new GlobalAttributeModel
                {
                    AttributeCode = entity.AttributeCode,
                    AttributeName = entity.ZnodeGlobalAttributeLocales.Count > 0 ? entity.ZnodeGlobalAttributeLocales?.Where(x => x.LocaleId == Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale)).Select(x => x.AttributeName)?.FirstOrDefault().ToString() : entity.AttributeCode,
                    GlobalAttributeId = entity.GlobalAttributeId,
                    AttributeTypeId = entity.AttributeTypeId,     
                    IsLocalizable = entity.IsLocalizable,
                    IsRequired = entity.IsRequired,
                    HelpDescription = entity.HelpDescription,
                    DisplayOrder = entity.DisplayOrder
                };
            }
            else
                return null;
        }

        public static GlobalAttributeGroupListModel AddGroupNameToListModel(IList<ZnodeGlobalAttributeGroup> entity)
        {
            if (IsNotNull(entity))
            {
                GlobalAttributeGroupListModel model = new GlobalAttributeGroupListModel();
                model.AttributeGroupList = (from _item in entity
                                            select new GlobalAttributeGroupModel
                                            {
                                                GlobalAttributeGroupId = _item.GlobalAttributeGroupId,
                                                GroupCode = _item.GroupCode,
                                                AttributeGroupName = _item.ZnodeGlobalAttributeGroupLocales.Count > 0 ? _item.ZnodeGlobalAttributeGroupLocales?.Where(x => x.LocaleId == Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale)).Select(x => x.AttributeGroupName).FirstOrDefault().ToString() : string.Empty,
                                                DisplayOrder = _item.DisplayOrder,
                                            }).ToList();
                return model;
            }
            else
                return null;
        }

        #endregion
    }
}
