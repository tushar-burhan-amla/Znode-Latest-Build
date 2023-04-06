using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class GlobalMediaDisplaySettingViewModel : BaseViewModel
    {
        public int GlobalMediaDisplaySettingsId { get; set; }
        public int? MediaId { get; set; }        
        public int? MaxDisplayItems { get; set; }       
        public string MediaPath { get; set; } = "";

        [Display(Name = ZnodePIM_Resources.LabelMaxCatalogItemSmallThumbnailWidth, ResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ImageRequiredField)]
        [Range(1, 1000, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.RangeValidatorNumbers)]
        [RegularExpression(AdminConstants.NumberValidation, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.MessageNumericValueAllowed)]
        public int? MaxSmallThumbnailWidth { get; set; } = ZnodeConstant.MaxSmallThumbnailWidth;

        [Display(Name = ZnodePIM_Resources.LabelMaxCatalogItemSmallWidth, ResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ImageRequiredField)]
        [Range(1, 1000, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.RangeValidatorNumbers)]
        [RegularExpression(AdminConstants.NumberValidation, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.MessageNumericValueAllowed)]
        public int? MaxSmallWidth { get; set; } = ZnodeConstant.MaxSmallWidth;

        [Display(Name = ZnodePIM_Resources.LabelMaxCatalogItemMediumWidth, ResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ImageRequiredField)]
        [Range(1, 1000, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.RangeValidatorNumbers)]
        [RegularExpression(AdminConstants.NumberValidation, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.MessageNumericValueAllowed)]
        public int? MaxMediumWidth { get; set; } = ZnodeConstant.MaxMediumWidth;

        [Display(Name = ZnodePIM_Resources.LabelMaxCatalogItemThumbnailWidth, ResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ImageRequiredField)]
        [Range(1, 1000, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.RangeValidatorNumbers)]
        [RegularExpression(AdminConstants.NumberValidation, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.MessageNumericValueAllowed)]
        public int? MaxThumbnailWidth { get; set; } = ZnodeConstant.MaxThumbnailWidth;

        [Display(Name = ZnodePIM_Resources.LabelMaxCatalogItemLargeWidth, ResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ImageRequiredField)]
        [Range(1, 1000, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.RangeValidatorNumbers)]
        [RegularExpression(AdminConstants.NumberValidation, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.MessageNumericValueAllowed)]
        public int? MaxLargeWidth { get; set; } = ZnodeConstant.MaxLargeWidth;

        [Display(Name = ZnodePIM_Resources.LabelMaxCatalogItemCrossSellWidth, ResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ImageRequiredField)]
        [Range(1, 1000, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.RangeValidatorNumbers)]
        [RegularExpression(AdminConstants.NumberValidation, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.MessageNumericValueAllowed)]
        public int? MaxCrossSellWidth { get; set; } = ZnodeConstant.MaxCrossSellWidth;
    }
}
