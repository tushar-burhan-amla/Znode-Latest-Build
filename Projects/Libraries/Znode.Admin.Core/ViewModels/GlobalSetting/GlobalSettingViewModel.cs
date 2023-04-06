using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class GlobalSettingViewModel : BaseViewModel
    {
        //DisplayUnit
        public int? DisplayUnitId { get; set; }
        public List<SelectListItem> DisplayUnitList { get; set; }

        //DateFormat
        [Required(ErrorMessageResourceType = typeof(GlobalSetting_Resources), ErrorMessageResourceName = ZnodeGlobalSetting_Resources.ErrorDateFormat)]
        public int? DateFormatId { get; set; }
        public List<SelectListItem> DateFormatList { get; set; }

        //TimeZone
        public int? TimeZoneId { get; set; }
        public List<SelectListItem> TimeZoneList { get; set; }

        //WeightUnit
        public int? WeightUnitId { get; set; }
        public List<SelectListItem> WeightUnitList { get; set; }

        //Price Round Off
        [Display(Name = ZnodeAdmin_Resources.LabelPriceRoundOff, ResourceType = typeof(Admin_Resources))]
        public int? PriceRoundOffFeatureValue { get; set; }
        public List<SelectListItem> PriceRoundOffList { get; set; }

        //Inventory Round Off
        [Display(Name = ZnodeAdmin_Resources.LabelInventoryRoundOff, ResourceType = typeof(Admin_Resources))]
        public int? InventoryRoundOffFeatureValue { get; set; }
        public List<SelectListItem> InventoryRoundOffList { get; set; }

        //Current Environment
        [Display(Name = ZnodeAdmin_Resources.LabelCurrentEnvironment, ResourceType = typeof(Admin_Resources))]
        public string CurrentEnvironmentFeatureValue { get; set; }
        public List<SelectListItem> EnvironmentsList { get; set; }

        //TimeFormat
        [Required(ErrorMessageResourceType = typeof(GlobalSetting_Resources), ErrorMessageResourceName = ZnodeGlobalSetting_Resources.ErrorTimeFormat)]
        public int? TimeFormatId { get; set; }
        public List<SelectListItem> TimeFormatList { get; set; }
        public string ServerTimeZone { get; set; }
    }
}