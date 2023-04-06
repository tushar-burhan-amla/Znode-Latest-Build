using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ContentContainerViewModel : BaseViewModel
    {
        
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ContainerKeyRequired)]
        [Display(Name = ZnodeAdmin_Resources.ContainerKey, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorAlphanumericOnly, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.ContainerKeyLimit, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string ContainerKey { get; set; }

        [MaxLength(1000)]
        [Display(Name = ZnodeAdmin_Resources.LabelTags, ResourceType = typeof(Admin_Resources))]
        public string Tags { get; set; }


        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredContainerFamily)]
        [Display(Name = ZnodeAdmin_Resources.ContainerFamily, ResourceType = typeof(Admin_Resources))]
        public string FamilyCode { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ContainerNameRequired)]
        [Display(Name = ZnodeAdmin_Resources.ContainerName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.ContainerNameLimit, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Name { get; set; }

        [Display(Name = ZnodeAdmin_Resources.ContainerTemplate, ResourceType = typeof(Admin_Resources))]   
        public string TemplateName { get; set; }
        public int? ContainerTemplateId { get; set; }
        public int PortalId { get; set; }
        public int ContentContainerId { get; set; }
        public int ContainerProfileVariantId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }

        public List<ContainerVariantViewModel> ContainerVariants { get; set; }

        public string FamilyName { get; set; }
        public List<SelectListItem> ContainerFamilies { get; set; }
        public List<SelectListItem> Variants { get; set; }
        public List<SelectListItem> ContainerTemplates { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public string PublishStatus { get; set; }
        public bool IsActive { get; set; } //This property is added in order to contain the active/deactive status of default variant while adding content container.
        public string TargetPublishState { get; set; } //This property is added in order to contain the target publish state of the container
        public string UrlEncodedContainerKey { get; set; }
        public string ConnectorTouchPoints { get; set; }
        public string SchedulerCallFor { get; set; }
    }
}
