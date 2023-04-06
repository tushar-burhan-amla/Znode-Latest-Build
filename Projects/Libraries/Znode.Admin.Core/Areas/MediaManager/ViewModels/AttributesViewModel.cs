using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;
namespace Znode.Engine.Admin.ViewModels
{
    public class AttributesViewModel : BaseViewModel
    {
        public TabViewListModel Tabs { get; set; } = null;
        public List<AttributeTypeModel> AttributeTypes { get; set; } = null;
        public int MediaAttributeId { get; set; }
        public int? AttributeTypeId { get; set; }
        public string AttributeValues { get; set; }
        public string AttributeName { get; set; }
        public string AttributeTypeName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterAttributeCode, ErrorMessageResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericStartWithAlphabet, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AttributeCodeLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string AttributeCode { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsConfigurable { get; set; }
        public bool? IsPersonalizable { get; set; }
        public bool? IsLocalizable { get; set; } = true;

        public int? AttributeValidationId { get; set; }
        public int? AttributeGroupId { get; set; }
        public bool? IsSystemDefined { get; set; }


        [Display(Name = ZnodeAttributes_Resources.LabelDisplayOrder, ResourceType = typeof(Attributes_Resources))]
        [Range(1, 999, ErrorMessageResourceName = ZnodeAttributes_Resources.ErrorDisplayOrder, ErrorMessageResourceType = typeof(Attributes_Resources))]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceName = ZnodeAttributes_Resources.InvalidDisplayOrder, ErrorMessageResourceType = typeof(Attributes_Resources))]
        public int? DisplayOrder { get; set; }

        public string HelpDescription { get; set; }
        public List<AttributesSelectValuesViewModel> SelectValues { get; set; }
    }
}