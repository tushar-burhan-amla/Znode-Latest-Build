using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class AttributesMap
    {

        public static AttributesInputValidationListModel ToInputValidationListModel(IList<ZnodeAttributeInputValidation> entity)
        {
            if (!Equals(entity, null))
            {
                AttributesInputValidationListModel model = new AttributesInputValidationListModel();
                model.InputValidations = new List<AttributeInputValidationDataModel>();

                model.InputValidations = (from _list in entity
                                          select new AttributeInputValidationDataModel()
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
                                                       select new InputValidationRuleDataModel
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


        public static AttributesDefaultValueListModel ToDefaultValueListModel(IList<ZnodeMediaAttributeDefaultValue> entity)
        {
            if (!Equals(entity, null))
            {
                AttributesDefaultValueListModel model = new AttributesDefaultValueListModel();
                model.DefaultValues = (from _item in entity
                                       select new AttributesDefaultValueModel
                                       {
                                           AttributeId = _item.MediaAttributeId,
                                           DefaultAttributeValueId = _item.MediaAttributeDefaultValueId,
                                           AttributeDefaultValueCode = _item.AttributeDefaultValueCode,
                                           ValueLocales = (from _locales in _item.ZnodeMediaAttributeDefaultValueLocales
                                                           select new DefaultAttributeValueLocaleModel
                                                           {
                                                               DefaultAttributeValue = _locales.DefaultAttributeValue,
                                                               LocaleId = _locales.LocaleId,
                                                               MediaAttributeDefaultValueLocaleId = _locales.MediaAttributeDefaultValueLocaleId,
                                                               MediaAttributeDefaultValueId = _locales.MediaAttributeDefaultValueId
                                                           }).ToList()
                                       }).ToList();
                return model;
            }
            return null;
        }

    }
}
