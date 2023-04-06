using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class AddonGroupViewModel : BaseViewModel
    {
        public int PimAddonGroupId { get; set; }

        [Display(Name = ZnodePIM_Resources.LabelDisplayType, ResourceType = typeof(PIM_Resources))]
        public string DisplayType { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorDisplayType)]
        [Display(Name = ZnodePIM_Resources.LabelDisplayType, ResourceType = typeof(PIM_Resources))]
        public AddonType DisplayTypeValue { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelName, ResourceType = typeof(Admin_Resources))]
        public string AddonGroupName { get; set; }

        public List<AddonGroupLocaleViewModel> PimAddonGroupLocales { get; set; }
        public ProductDetailsListViewModel AssociatedChildProducts { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }
        public int PimAddonProductId { get; set; }

        public List<AddOnProductViewModel> PimAddOnProducts { get; set; }
    }
}