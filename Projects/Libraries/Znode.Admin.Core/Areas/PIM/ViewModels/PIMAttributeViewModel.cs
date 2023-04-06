using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PIMAttributeViewModel : BaseViewModel
    {
        public TabViewListModel Tabs { get; set; } = null;
        public List<AttributeTypeModel> AttributeTypes { get; set; } = null;
        public int PimAttributeId { get; set; }
        public int? AttributeTypeId { get; set; }

        public string AttributeTypeName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterAttributeCode, ErrorMessageResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericStartWithAlphabet, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AttributeCodeLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string AttributeCode { get; set; }
        public bool? IsRequired { get; set; } = true;
        public bool? IsConfigurable { get; set; }
        public bool? IsPersonalizable { get; set; }
        public bool? IsLocalizable { get; set; } = true;

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterDisplayOrder, ErrorMessageResourceType = typeof(Admin_Resources))]
        public int? AttributeDisplayOrder { get; set; }
        public int? AttributeValidationId { get; set; }
        public int? AttributeGroupId { get; set; }
        public bool? IsSystemDefined { get; set; }
        public string HelpDescription { get; set; }

        [Display(Name = ZnodePIM_Resources.LabelDisplayOrder, ResourceType = typeof(PIM_Resources))]
        [Range(1, 999, ErrorMessageResourceName = ZnodePIM_Resources.ErrorDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceName = ZnodePIM_Resources.InvalidDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        public int? DisplayOrder { get; set; }
        public bool IsCategory { get; set; }
        public string AttributeDefaultValue { get; set; }
        public int? AttributeDefaultValueId { get; set; }
        public string AttributeName { get; set; }
        public bool? IsShowOnGrid { get; set; }
        public bool? IsSwatchImage { get; set; }
        public bool IsComparable { get; set; }
        public bool IsUseInSearch { get; set; }
        public bool IsFacets { get; set; }
        public int UsedInProductsCount { get; set; }
    }
}