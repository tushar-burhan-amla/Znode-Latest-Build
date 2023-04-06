using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class ContainerVariantDataViewModel : BaseViewModel
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public List<GlobalAttributeGroupViewModel> Groups { get; set; }
        public List<GlobalAttributeValuesViewModel> Attributes { get; set; }
        public int ProfileVariantId { get; set; }
        public int? LocaleId { get; set; }
        public int? ContainerTemplateId { get; set; }
        public List<SelectListItem> Locales { get; set; }
        public List<SelectListItem> ContainerTemplates { get; set; }
        public string FamilyCode { get; set; }
        public string StoreName { get; set; }
        public string ProfileName { get; set; }
        public string ContainerKey { get; set; }
        public string ContainerName { get; set; }
        public GlobalAttributeEntityDetailsViewModel entityAttributeModel { get; set; }
        public bool IsActive { get; set; } //This property is added in order to contain the active/deactive status of variants
        public string TargetPublishState { get; set; } //This property is added in order to contain the target publish state of the variant
        public bool IsDataAddedForDefaultLocale { get; set; } //This property is added in order to check if the global attributes data is added in specified variant for default locale or not.
    }
}
