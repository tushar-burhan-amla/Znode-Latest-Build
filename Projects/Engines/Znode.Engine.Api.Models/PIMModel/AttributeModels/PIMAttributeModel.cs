using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;
namespace Znode.Engine.Api.Models
{
    public class PIMAttributeModel : BaseModel
    {
        public int PimAttributeId { get; set; }
        public Nullable<int> ParentPimAttributeId { get; set; }
        public int? AttributeTypeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeTypeName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeApi_Resources.PleaseEnterAttributeCode, ErrorMessageResourceType = typeof(Api_Resources))]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeApi_Resources.AlphaNumericOnly, ErrorMessageResourceType = typeof(Api_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeApi_Resources.AttributeCodeLength, ErrorMessageResourceType = typeof(Api_Resources))]
        public string AttributeCode { get; set; }
        public bool IsRequired { get; set; } = true;
        public bool IsLocalizable { get; set; } = true;
        public bool IsFilterable { get; set; } = true;
        public bool IsSystemDefined { get; set; }
        public bool IsPersonalizable { get; set; } = true;
        public bool IsConfigurable { get; set; } = true;
        public int? AttributeGroupId { get; set; }
        public int? AttributeDisplayOrder { get; set; }
        public string HelpDescription { get; set; }
        public string AttributeType { get; set; }
        public bool IsComparable { get; set; }
        public bool IsUseInSearch { get; set; }
        public bool IsFacets { get; set; }

        [Range(1, 999, ErrorMessageResourceName = ZnodePIM_Resources.ErrorDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        [RegularExpression("^([0-9][0-9]*)$", ErrorMessageResourceName = ZnodePIM_Resources.InvalidDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        public int? DisplayOrder { get; set; }
        public bool IsCategory { get; set; }
        public string AttributeDefaultValue { get; set; }
        public int? AttributeDefaultValueId { get; set; }
        public List<PIMAttributeLocaleModel> Locales { get; set; }
        public bool IsHidden { get; set; }
        public bool? IsShowOnGrid { get; set; }
        public bool? IsSwatch { get; set; }
        public int UsedInProductsCount { get; set; }

        public PIMAttributeModel()
        {
            Locales = new List<PIMAttributeLocaleModel>();
        }
    }
}
