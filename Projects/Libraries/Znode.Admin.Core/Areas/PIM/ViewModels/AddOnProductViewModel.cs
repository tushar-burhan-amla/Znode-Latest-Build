using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class AddOnProductViewModel : BaseViewModel
    {
        public int PimAddOnProductId { get; set; }
        public int? PimProductId { get; set; }
        public int? PimAddonGroupId { get; set; }

        [Display(Name = ZnodePIM_Resources.LabelDisplayOrder, ResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.RequiredDisplayOrder)]
        [Range(1, 999, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorDisplayOrder)]
        public int? DisplayOrder { get; set; } = 500;

        public int? AddOnDisplayOrder { get; set; }

        [Display(Name = ZnodePIM_Resources.RequiredType  , ResourceType = typeof(PIM_Resources))]
        public RequiredType RequiredTypeValue { get; set; }
        [Display(Name = ZnodePIM_Resources.RequiredType, ResourceType = typeof(PIM_Resources))]
        public string RequiredType { get; set; }

        public AddonGroupViewModel ZnodePimAddonGroup { get; set; }
    }
}