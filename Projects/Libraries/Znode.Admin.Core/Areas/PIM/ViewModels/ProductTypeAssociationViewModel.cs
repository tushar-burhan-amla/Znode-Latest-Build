using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProductTypeAssociationViewModel : BaseViewModel
    {
        public int PimProductTypeAssociationId { get; set; }
        public int? PimParentProductId { get; set; }
        public int? PimProductId { get; set; }
        public int? PimAttributeId { get; set; }
        public string AttributeCode { get; set; }

        [Display(Name = ZnodePIM_Resources.LabelDisplayOrder, ResourceType = typeof(PIM_Resources))]
        [Range(1, 999, ErrorMessageResourceName = ZnodePIM_Resources.ErrorDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceName = ZnodePIM_Resources.InvalidDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceName = ZnodePIM_Resources.RequiredDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        public int? DisplayOrder { get; set; } = 1;
        public bool IsDefault { get; set; }
        public decimal? BundleQuantity { get; set; } = 1;
    }
}