using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Agents;

namespace Znode.Engine.WebStore.Maps
{
    public static class ImportViewModelMap
    {
        public static ImportModel ToModel(string fileName, ImportModel importModel)
        {
            return new ImportModel()
            {
                LocaleId = PortalAgent.CurrentPortal.LocaleId,
                TemplateId = importModel.TemplateId,
                TemplateName = importModel.TemplateName,
                Mappings = new ImportMappingListModel { DataMappings = GetDataMappings(importModel.TemplateMappingList) },
                FileName = fileName,
                ImportType = importModel.ImportType,
                PortalId = PortalAgent.CurrentPortal.PortalId,
                IsAccountAddress = true
            };
        }

        public static List<ImportMappingModel> GetDataMappings(ImportTemplateMappingListModel importMappingTemplateList)
        {
            List<ImportMappingModel> dataMappings = new List<ImportMappingModel>();

            foreach (ImportTemplateMappingModel map in importMappingTemplateList.Mappings ?? new List<ImportTemplateMappingModel>())
            {
                dataMappings.Add(new ImportMappingModel { MapCsvColumn = map.SourceColumnName, MapTargetColumn = map.TargetColumnName });
            }
            return dataMappings ?? new List<ImportMappingModel>();
        }
    }
}
