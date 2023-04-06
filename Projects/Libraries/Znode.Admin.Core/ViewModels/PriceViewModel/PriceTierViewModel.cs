using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceTierViewModel : BaseViewModel
    {
        public int? PriceTierId { get; set; }
        public int? PriceListId { get; set; }

        [MaxLength(300, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSKURange)]
        public string SKU { get; set; }

        [Range(0, 999999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PriceRangeValidatorNumbers)]
        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorPriceRange)]
        public decimal? Price { get; set; }

        [Range(1, 999999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.QtyRangeValidatorNumbers)]
        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.SelectedQuantity)]
        [Display(Name = ZnodeAdmin_Resources.LabelMinQuantityCode, ResourceType = typeof(Admin_Resources))]
        public decimal? Quantity { get; set; }

        [Range(0, 999999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PriceRangeValidatorNumbers)]
        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorPriceRange)]
        [Display(Name = ZnodeAdmin_Resources.LabelPriceCustom1, ResourceType = typeof(Admin_Resources))]
        public string Custom1 { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPriceCustom1, ResourceType = typeof(Admin_Resources))]
        public string Custom2 { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPriceCustom1, ResourceType = typeof(Admin_Resources))]
        public string Custom3 { get; set; }

        

    }
}