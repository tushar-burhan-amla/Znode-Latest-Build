using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PIMAttributeGroupViewModel : BaseViewModel
    {
        public int PimAttributeGroupId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodePIM_Resources.RequiredGroupCode, ErrorMessageResourceType = typeof(PIM_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericStartWithAlphabet, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelGroupCode, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AttributeGroupCodeLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string GroupCode { get; set; }
        public bool IsSystemDefined { get; set; }
        public string AttributeGroupName { get; set; }
        public bool IsCategory { get; set; }
        public int? DisplayOrder { get; set; }
        public int PimAttributeFamilyId { get; set; }
        public bool IsNonEditable { get; set; }
        public List<PIMAttributeModel> Attributes { get; set; }
        public string GroupType { get; set; }
    }
}