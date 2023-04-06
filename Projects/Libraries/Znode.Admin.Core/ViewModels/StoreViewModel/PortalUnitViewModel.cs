using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalUnitViewModel : BaseViewModel
    {
        public int PortalUnitId { get; set; }
        public int? PortalId { get; set; }
        public int OldCurrencyId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCurrency, ResourceType = typeof(Admin_Resources))]
        public int? CurrencyTypeID { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelWeightUnit, ResourceType = typeof(Admin_Resources))]
        public string WeightUnit { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelDimensionUnit, ResourceType = typeof(Admin_Resources))]
        public string DimensionUnit { get; set; }

        [MaxLength(50, ErrorMessageResourceName = ZnodeAdmin_Resources.CurrencySuffixMaxLengthMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string CurrencySuffix { get; set; }

        public int? CultureId { get; set; }
        public int OldCultureId { get; set; }

        public string CurrencyPreview { get; set; }
        public string CurrencyName { get; set; }
        public string PortalName { get; set; }

        public List<SelectListItem> WeightUnits { get; set; }
        public List<SelectListItem> DimensionUnits { get; set; }
        public List<SelectListItem> CurrencyTypes { get; set; }
        public List<SelectListItem> Culture { get; set; }
    }
}