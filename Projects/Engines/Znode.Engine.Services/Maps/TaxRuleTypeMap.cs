using Znode.Engine.Api.Models;
using Znode.Engine.Taxes.Interfaces;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class TaxRuleTypeMap
    {
        public static TaxRuleTypeModel ToTaxRuleTypeModel(ZnodeTaxRuleType taxRuleTypeEntity)
        {
            if (Equals(taxRuleTypeEntity, null))
                return null;

            TaxRuleTypeModel taxRuleTypemodel = new TaxRuleTypeModel
            {
                TaxRuleTypeId = taxRuleTypeEntity.TaxRuleTypeId,
                ClassName = taxRuleTypeEntity.ClassName,
                Name = taxRuleTypeEntity.Name,
                IsActive = taxRuleTypeEntity.IsActive,
                Description = taxRuleTypeEntity.Description,
                PortalId = taxRuleTypeEntity.PortalId,
            };
            return taxRuleTypemodel;
        }

        public static ZnodeTaxRuleType ToTaxRuleTypeEntity(TaxRuleTypeModel taxRuleTypeModel)
        {
            if (Equals(taxRuleTypeModel, null))
                return null;

            ZnodeTaxRuleType taxRuleTypeEntity = new ZnodeTaxRuleType
            {
                TaxRuleTypeId = taxRuleTypeModel.TaxRuleTypeId,
                ClassName = taxRuleTypeModel.ClassName,
                Name = taxRuleTypeModel.Name,
                IsActive = taxRuleTypeModel.IsActive,
                Description = taxRuleTypeModel.Description,
                PortalId = taxRuleTypeModel.PortalId
            };
            return taxRuleTypeEntity;
        }

        public static TaxRuleTypeModel ToModel(IZnodeTaxesType znodeTaxType)
        {
            if (Equals(znodeTaxType, null))
                return null;

            TaxRuleTypeModel taxRuletypeModel = new TaxRuleTypeModel
            {
                ClassName = znodeTaxType.ClassName,
                Description = znodeTaxType.Description,
                Name = znodeTaxType.Name,
            };
            return taxRuletypeModel;
        }
    }
}
