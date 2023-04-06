using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    //This is model for Import feature.
    public class ImportModel : BaseModel
    {
        public ImportModel()
        {
            TemplateList = new ImportTemplateListModel();
            ImportTypeList = new ImportTypeListModel();
            TemplateMappingList = new ImportTemplateMappingListModel();
            FamilyList = new ImportProductFamilyListModel();
        }

        [Required]
        public string FileName { get; set; }
        [Required]
        public string ImportType { get; set; }
        [Required]

        public ImportTemplateListModel TemplateList { get; set; }
        public ImportTypeListModel ImportTypeList { get; set; }
        public ImportTemplateModel SelectedTemplate { get; set; }
        public ImportTemplateMappingListModel TemplateMappingList { get; set; }
        public ImportProductFamilyListModel FamilyList { get; set; }

        public int ImportTypeId { get; set; }
        public int LocaleId { get; set; }
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
        public ImportMappingListModel Mappings { get; set; }
        public int? FamilyId { get; set; }
        public bool IsPartialPage { get; set; }
        public int? PriceListId { get; set; }
        public string CountryCode { get; set; }
        public int? PortalId { get; set; }
        public bool IsAccountAddress { get; set; } = false;
        public bool IsAutoPublish { get; set; } = false;
        public int? CatalogId { get; set; }
        public int? PromotionTypeId { get; set; }
    }
}
