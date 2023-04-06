using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class AddonGroupLocaleViewModel : BaseViewModel
    {
        public int PimAddonGroupLocaleId { get; set; }
        public int? PimAddonGroupId { get; set; }

        [Display(Name =ZnodeAdmin_Resources.LabelName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorAddonGroupNameRequired)]
        public string AddonGroupName { get; set; }

        public int? LocaleId { get; set; }

        public AddonGroupViewModel ZnodePimAddonGroup { get; set; }
    }
}