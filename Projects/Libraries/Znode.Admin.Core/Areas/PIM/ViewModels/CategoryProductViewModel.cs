using System;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CategoryProductViewModel : BaseViewModel
    {
        public int PimCategoryProductId { get; set; }
        public int PimCategoryId { get; set; }
        public int PimProductId { get; set; }

        [Display(Name = ZnodePIM_Resources.LabelDisplayOrder, ResourceType = typeof(PIM_Resources))]
        [Range(1, 999, ErrorMessageResourceName = ZnodePIM_Resources.ErrorDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceName = ZnodePIM_Resources.InvalidDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceName = ZnodePIM_Resources.RequiredDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        public int DisplayOrder { get; set; } = ZnodeConstant.DisplayOrder;
        public bool Status { get; set; }
        public string ProductName { get; set; }
        public string ImagePath { get; set; }
        public string ProductType { get; set; }
        public string AttributeFamily { get; set; }
        public string SKU { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Assortment { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
    }
}