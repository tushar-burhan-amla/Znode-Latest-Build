using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ImportViewModel : BaseViewModel
    {
        [Display(Name = ZnodeAdmin_Resources.LabelImportType, ResourceType = typeof(Admin_Resources))]
        public int ImportHeadId { get; set; }
        public List<SelectListItem> ImportTypeList { get; set; }
        [FileTypeValidation(AdminConstants.CSVFileType, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.CSVFileTypeErrorMessage)]
        [Display(Name = ZnodeAdmin_Resources.LabelFileName, ResourceType = typeof(Admin_Resources))]
        [UIHint("FileUploader")]
        public HttpPostedFileBase FilePath { get; set; }
        [Display(Name = ZnodeAdmin_Resources.TemplateId, ResourceType = typeof(Admin_Resources))]
        public int TemplateId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelImportType, ResourceType = typeof(Admin_Resources))]
        public string TemplateType { get; set; }
        public List<SelectListItem> TemplateTypeList { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelImportTemplateName, ResourceType = typeof(Admin_Resources))]
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
        public List<SelectListItem> TemplatesMappings { get; set; }
        public int LocaleId { get; set; }
        public List<ImportMappingsViewModel> Mappings { get; set; }
        public string FileName { get; set; }
        public string ImportType { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelFamilyList, ResourceType = typeof(Admin_Resources))]
        public List<SelectListItem> FamilyList { get; set; }
        public int? FamilyId { get; set; }
        public string ListCode { get; set; }
        public List<SelectListItem> PricingList { get; set; }
        public List<SelectListItem> CatalogList { get; set; }
        public bool IsPartialPage { get; set; }
        public int? PriceListId { get; set; }
        public string CountryCode { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> PortalList { get; set; }
        public int? PortalId { get; set; }
        public bool IsAutoPublish { get; set; }
        public string StoreName { get; set; }
        public int? CatalogId { get; set; }
        public List<SelectListItem> PromotionTypeList { get; set; }
        public int? PromotionTypeId { get; set; }
    }
}