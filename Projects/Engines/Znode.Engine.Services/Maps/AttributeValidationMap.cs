using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class AttributeValidationMap
    {
        //Convert ZnodeAttributeValidation entity in to AttributeValidationDataModel model.
        public static AttributeValidationDataModel ToModel(ZnodeMediaAttributeValidation entity)
        {
            if (!Equals(entity, null))
            {
                return new AttributeValidationDataModel
                {
                    AttributeValidationId = entity.MediaAttributeValidationId,
                    AttributeId = entity.MediaAttributeId,
                    InputValidationId = entity.InputValidationId,
                    InputValidationRuleId = entity.InputValidationRuleId,
                    Name = entity.Name,
                    CreatedBy= entity.CreatedBy,
                    CreatedDate = entity.CreatedDate,
                    ModifiedBy= entity.ModifiedBy,
                    ModifiedDate= entity.ModifiedDate
                   
                };
            }
            else
                return null;
        }
        //Convert AttributeValidationDataModel model in to ZnodeAttributeValidation entity.
        public static ZnodeMediaAttributeValidation ToEntity(AttributeValidationDataModel model)
        {
            if (!Equals(model, null))
            {
                return new ZnodeMediaAttributeValidation
                {
                    MediaAttributeValidationId = model.AttributeValidationId,
                    MediaAttributeId = model.AttributeId,
                    InputValidationId = model.InputValidationId,
                    InputValidationRuleId = model.InputValidationRuleId,
                    Name = model.Name,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = model.CreatedDate,
                    ModifiedBy = model.ModifiedBy,
                    ModifiedDate = model.ModifiedDate
                };
            }
            else
                return null;
        }
        //Convert IList<ZnodeAttributeValidation> in to AttributeValidationListModel model.
        public static AttributeValidationListModel ToListModel(IEnumerable<ZnodeMediaAttributeValidation> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new AttributeValidationListModel();
                foreach (var item in entity)
                {
                    model.AttributeValidationDataModelList.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }
        //Convert AttributeValidationListModel in to IList<ZnodeAttributeValidation> model.
        public static IList<ZnodeMediaAttributeValidation> ToListEntity(AttributeValidationListModel entity)
        {
            if (!Equals(entity, null))
            {
                List<ZnodeMediaAttributeValidation> model = new List<ZnodeMediaAttributeValidation>();
                foreach (var item in entity.AttributeValidationDataModelList)
                {
                    model.Add(ToEntity(item));
                }
                return model;
            }
            else
                return null;
        }


    }
}
