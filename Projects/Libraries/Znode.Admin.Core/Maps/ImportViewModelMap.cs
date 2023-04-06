using System;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Maps
{
    public static class ImportViewModelMap
    {
        public static ImportModel ToModel(ImportViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                return new ImportModel()
                {
                    LocaleId = model.LocaleId,
                    ImportTypeId = model.ImportHeadId,
                    TemplateId = model.TemplateId,
                    TemplateName = model.TemplateName,
                    TemplateVersion = model.TemplateVersion,
                    Mappings = new ImportMappingListModel { DataMappings = model.Mappings?.ToModel<ImportMappingModel>().ToList() },
                    FileName = model.FileName,
                    ImportType = model.ImportType,
                    FamilyId = model.FamilyId,
                    IsPartialPage = model.IsPartialPage,
                    PriceListId = model.PriceListId,
                    CountryCode = model.CountryCode,
                    PortalId = model.PortalId,
                    IsAutoPublish = model.IsAutoPublish,
                    CatalogId = model.CatalogId,
                    PromotionTypeId = model.PromotionTypeId
                };
            }
            return null;
        }
    }

}