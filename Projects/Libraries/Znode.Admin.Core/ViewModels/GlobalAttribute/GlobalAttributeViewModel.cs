using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;
namespace Znode.Engine.Admin.ViewModels
{
    public class GlobalAttributeViewModel : BaseViewModel
    {
        public TabViewListModel Tabs { get; set; } = null;
        public List<AttributeTypeModel> AttributeTypes { get; set; } = null;
        public int GlobalAttributeId { get; set; }
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
        public bool? IsSystemDefined { get; set; }
        public string HelpDescription { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelDisplayOrder, ResourceType = typeof(Admin_Resources))]
        [Range(1, 999, ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorDisplayOrder, ErrorMessageResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidDisplayOrder, ErrorMessageResourceType = typeof(Admin_Resources))]
        public int? DisplayOrder { get; set; }
        public string AttributeDefaultValue { get; set; }
        public int? AttributeDefaultValueId { get; set; }
        public string AttributeName { get; set; }
        public bool? IsSwatchImage { get; set; }
        public int GlobalEntityId { get; set; }
        public string EntityName { get; set; }
        public List<SelectListItem> GlobalEntityType { get; set; }
    }
}
