using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class PIMAttributesMap
    {
        #region Public Methods

        //Convert inputvalidation entity model to input validation list model
        public static PIMAttributeLocaleListModel ToLocaleListModel(IList<ZnodePimAttributeLocale> entity)
        {
            if (!Equals(entity, null))
            {
                PIMAttributeLocaleListModel model = new PIMAttributeLocaleListModel();
                model.Locales = (from _item in entity
                                 select new PIMAttributeLocaleModel
                                 {
                                     LocaleId = _item.LocaleId,
                                     PimAttributeId = _item.PimAttributeId,
                                     PimAttributeLocaleId = _item.PimAttributeLocaleId,
                                     AttributeName = _item.AttributeName
                                 }).ToList();
                return model;
            }
            return null;
        }

        public static PIMAttributeDefaultValueListModel ToDefaultValueListModel(IList<ZnodePimAttributeDefaultValue> entity)
        {
            if (!Equals(entity, null))
            {
                PIMAttributeDefaultValueListModel model = new PIMAttributeDefaultValueListModel();
                model.DefaultValues = (from _item in entity
                                       select new PIMAttributeDefaultValueModel
                                       {
                                           PimAttributeId = _item.PimAttributeId,
                                           PimDefaultAttributeValueId = _item.PimAttributeDefaultValueId,
                                           AttributeDefaultValueCode = _item.AttributeDefaultValueCode,
                                           DisplayOrder = _item.DisplayOrder,
                                           IsDefault = _item.IsDefault,
                                           MediaId = _item.MediaId ?? 0,
                                           SwatchText = _item.SwatchText,
                                           ValueLocales = (from _locales in _item.ZnodePimAttributeDefaultValueLocales
                                                           select new PIMAttributeDefaultValueLocaleModel
                                                           {
                                                               DefaultAttributeValue = _locales.AttributeDefaultValue,
                                                               LocaleId = _locales.LocaleId,
                                                               PimAttributeDefaultValueLocaleId = _locales.PimAttributeDefaultValueLocaleId,
                                                               PimDefaultAttributeValueId = _locales.PimAttributeDefaultValueId
                                                           }).ToList()
                                       }).ToList();
                return model;
            }
            return null;
        }

        public static PIMAttributeInputValidationListModel ToInputValidationListModel(IList<ZnodeAttributeInputValidation> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new PIMAttributeInputValidationListModel();
                model.InputValidations = new List<PIMAttributeInputValidationModel>();

                model.InputValidations = (from _list in entity
                                          select new PIMAttributeInputValidationModel()
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
                                                       select new PIMAttributeValidationRuleModel
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

        //Map Frontproperties Entity Model to Frontpropertiesmodel
        public static PIMFrontPropertiesModel ToFrontPropertiesModel(ZnodePimFrontendProperty entity)
        {
            PIMFrontPropertiesModel model = new PIMFrontPropertiesModel();
            if (!Equals(entity, null))
            {
                model.IsComparable = entity.IsComparable;
                model.IsAllowHtmlTag = entity.IsHtmlTags;
                model.IsUseInSearch = entity.IsUseInSearch;
                model.IsFacets = entity.IsFacets;
            }
            return model;
        }

        //Map PimAttribute Entity Model to AttributeListModel
        public static PIMAttributeListModel ToListModel(IList<ZnodePimAttribute> entity)
        {
            if (!Equals(entity, null))
            {
                PIMAttributeListModel model = new PIMAttributeListModel();
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
        public static PIMAttributeModel ToModel(ZnodePimAttribute entity)
        {
            if (!Equals(entity, null))
            {
                return new PIMAttributeModel
                {
                    AttributeCode = entity.AttributeCode,
                    AttributeName = entity.ZnodePimAttributeLocales.Count > 0 ? entity.ZnodePimAttributeLocales?.Where(x => x.LocaleId == Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale)).Select(x => x.AttributeName)?.FirstOrDefault().ToString() : entity.AttributeCode,
                    PimAttributeId = entity.PimAttributeId,
                    AttributeTypeId = entity.AttributeTypeId,
                    IsFilterable = entity.IsFilterable,
                    IsLocalizable = entity.IsLocalizable,
                    IsRequired = entity.IsRequired,
                    IsSystemDefined = entity.IsSystemDefined,
                    HelpDescription = entity.HelpDescription,
                    DisplayOrder = entity.DisplayOrder,
                    IsShowOnGrid = entity.IsShowOnGrid
                };
            }
            else
                return null;
        }


        //Map frontproperties model to frontproperties entity model
        public static ZnodePimFrontendProperty ToFrontPropertiesEntity(PIMFrontPropertiesModel frontProperties, int Id)
        {
            if (!Equals(frontProperties, null))
            {
                return new ZnodePimFrontendProperty
                {
                    ZnodePimFrontendPropertiesId = Id,
                    PimAttributeId = frontProperties.PimAttributeId,
                    IsComparable = Convert.ToBoolean(frontProperties.IsComparable),
                    IsFacets = Convert.ToBoolean(frontProperties.IsFacets),
                    IsHtmlTags = Convert.ToBoolean(frontProperties.IsAllowHtmlTag),
                    IsUseInSearch = Convert.ToBoolean(frontProperties.IsUseInSearch)
                };
            }
            else
                return null;
        }

        //Map PIMAttributeValidationModel to PimAttributeValidation entity model
        public static ZnodePimAttributeValidation ToValidationEntity(PIMAttributeValidationModel model, string attributeType)
        {
            if (!Equals(model, null))
            {
                return new ZnodePimAttributeValidation
                {
                    InputValidationRuleId = model.InputValidationRuleId,
                    InputValidationId = model.InputValidationId,
                    Name = attributeType.Equals(ZnodeConstant.DateType) ? HelperUtility.ConvertStringToSqlDateFormat(model.Name) : model.Name,
                    PimAttributeId = model.PimAttributeId,
                    PimAttributeValidationId = model.PimAttributeValidationId
                };
            }
            else return null;
        }

        //Map PimAttribute entity model to AttributeModel
        public static PIMAttributeModel ToAttributeModel(ZnodePimAttribute entity)
        {
            if (!Equals(entity, null))
            {
                return new PIMAttributeModel
                {
                    AttributeTypeId = entity.AttributeTypeId,
                    AttributeCode = entity.AttributeCode,
                    PimAttributeId = entity.PimAttributeId,
                    IsFilterable = entity.IsFilterable,
                    IsLocalizable = entity.IsLocalizable,
                    IsRequired = entity.IsRequired,
                    IsConfigurable = entity.IsConfigurable,
                    IsPersonalizable = entity.IsPersonalizable,
                    HelpDescription = entity.HelpDescription,
                    DisplayOrder = entity.DisplayOrder,
                    IsSystemDefined = entity.IsSystemDefined,
                    IsShowOnGrid = entity.IsShowOnGrid,
                    IsSwatch = entity.IsSwatch,
                    Locales = (from _locale in entity.ZnodePimAttributeLocales
                               select new PIMAttributeLocaleModel
                               {
                                   LocaleId = _locale.LocaleId,
                                   AttributeName = _locale.AttributeName,
                                   PimAttributeLocaleId = _locale.PimAttributeLocaleId                        
            }).ToList()
                };
            }
            else
                return null;
        }

        public static ZnodePimAttributeDefaultValue ToDefaultValueEntity(PIMAttributeDefaultValueModel model)
        {
            if (!Equals(model, null))
            {
                return new ZnodePimAttributeDefaultValue()
                {
                    PimAttributeId = model.PimAttributeId,
                    AttributeDefaultValueCode = model.AttributeDefaultValueCode,
                    DisplayOrder = model.DisplayOrder,
                    IsDefault = model.IsDefault,
                    //IsSwatch = model.IsSwatch,
                    MediaId = Equals(model.IsSwatch, true) ? Convert.ToInt32(model.SwatchText) : model.MediaId,
                    SwatchText = !Equals(model.IsSwatch, true) ? model.SwatchText : string.Empty,
                };
            }
            return null;
        }

        //Convert Into PimAttributeDefaultValue model 
        public static ZnodePimAttributeDefaultValueLocale ToDefaultValueLocaleEntity(PIMAttributeDefaultValueLocaleModel model)
        {
            if (!Equals(model, null))
            {
                return new ZnodePimAttributeDefaultValueLocale()
                {
                    LocaleId = model.LocaleId,
                    AttributeDefaultValue = model.DefaultAttributeValue,
                    PimAttributeDefaultValueId = model.PimDefaultAttributeValueId

                };
            }
            return null;
        }

        //Map attribute and frontproperties entity model to attribute data model
        public static PIMAttributeDataModel ToPIMAttributeDataModel(ZnodePimAttribute entity, ZnodePimFrontendProperty Propertyentity)
        {
            return new PIMAttributeDataModel()
            {
                AttributeModel = ToAttributeModel(entity),
                FrontProperties = ToFrontPropertiesModel(Propertyentity)
            };
        }

        public static PIMAttributeDefaultValueModel ToPIMAttributeDefaultValueModel(PIMAttributeDataModel model, int defaultLocaleId, string attributeType)
        {
            return new PIMAttributeDefaultValueModel()
            {
                PimDefaultAttributeValueId = model.AttributeModel.AttributeDefaultValueId.GetValueOrDefault(),
                ValueLocales = ToPIMAttributeDefaultValueLocaleModelList(model, defaultLocaleId, attributeType),
                PimAttributeId = model.AttributeModel.PimAttributeId
            };
        }
        public static List<PIMAttributeDefaultValueLocaleModel> ToPIMAttributeDefaultValueLocaleModelList(PIMAttributeDataModel model, int defaultLocaleId, string attributeType)
        {
            List<PIMAttributeDefaultValueLocaleModel> modelList = new List<PIMAttributeDefaultValueLocaleModel>();
            modelList.Add(new PIMAttributeDefaultValueLocaleModel()
            {
                PimDefaultAttributeValueId = model.AttributeModel.AttributeDefaultValueId.GetValueOrDefault(),
                DefaultAttributeValue = attributeType.Equals(ZnodeConstant.DateType) ? HelperUtility.ConvertStringToSqlDateFormat(model.AttributeModel.AttributeDefaultValue) : model.AttributeModel.AttributeDefaultValue,
                LocaleId = defaultLocaleId
            });
            return modelList;
        }

        #endregion
    }
}
