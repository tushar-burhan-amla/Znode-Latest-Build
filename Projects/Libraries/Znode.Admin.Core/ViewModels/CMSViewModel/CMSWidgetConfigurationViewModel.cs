using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CMSWidgetConfigurationViewModel : BaseViewModel
    {
        public int CMSWidgetSliderBannerId { get; set; }
        public int CMSWidgetsId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredWidgetSlider)]
        [Display(Name = ZnodeAdmin_Resources.TextSelectSlider, ResourceType = typeof(Admin_Resources))]
        public int CMSSliderId { get; set; }
        public int PortalId { get; set; }
        public int CMSMappingId { get; set; }

        [RegularExpression(@"^(?:[1-9][0-9]{3}|[1-9][0-9]{2}|[1-9][0-9]|[1-9])$", ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorAutoPlayTimeOutRange, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.TextAutoPlayTimeout, ResourceType = typeof(Admin_Resources))]
        public int? AutoplayTimeOut { get; set; }
        public int? CMSContentPagesId { get; set; }

        public string WidgetCode { get; set; }
        [Display(Name = ZnodeAdmin_Resources.TextSelectType, ResourceType = typeof(Admin_Resources))]
        public string Type { get; set; }
        [Display(Name = ZnodeAdmin_Resources.TextSelectNavigation, ResourceType = typeof(Admin_Resources))]
        public string Navigation { get; set; }
        [Display(Name = ZnodeAdmin_Resources.TextSelectTransitionStyle, ResourceType = typeof(Admin_Resources))]
        public string TransactionStyle { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOFMapping { get; set; }
        public string PortalName { get; set; }
        public string DisplayName { get; set; }
        public string WidgetName { get; set; }
        public string FileName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TextAutoPlay, ResourceType = typeof(Admin_Resources))]
        public bool AutoPlay { get; set; }
        [Display(Name = ZnodeAdmin_Resources.TextAutoplayHoverPause, ResourceType = typeof(Admin_Resources))]
        public bool AutoplayHoverPause { get; set; }

        public List<SelectListItem> CMSPortalSliderList { get; set; }
        public List<SelectListItem> SliderTypeList { get; set; }
        public List<SelectListItem> SliderNavigationTypeList { get; set; }
        public List<SelectListItem> SliderTransitionTypeList { get; set; }
        public bool EnableCMSPreview { get; set; }
        public int LocaleId { get; set; }

    }
}