using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProviderEngineViewModel : BaseViewModel
    {
        [MaxLength(50)]
        [Display( Name = ZnodeAdmin_Resources.LabelClassName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredClassName)]
        public string ClassName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredName)]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredDescription)]
        public string Description { get; set; }
        public string ClassType { get; set; }
        public string ProviderEngineClass { get; set; }

        public int Id { get; set; }

        public bool IsActive { get; set; }

        public int? PortalId { get; set; }

        public List<SelectListItem> ProviderEngineClasses { get; set; }
    }
}