using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class GlobalAttributeGroupViewModel : BaseViewModel
    {
        public int GlobalAttributeGroupId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredAttributeGroup, ErrorMessageResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericStartWithAlphabet, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelGroupCode, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AttributeGroupCodeLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string GroupCode { get; set; }
        public string AttributeGroupName { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsNonEditable { get; set; }
        public List<GlobalAttributeModel> Attributes { get; set; }
        public string GroupType { get; set; }
        public int GlobalEntityId { get; set; }
        public List<SelectListItem> GlobalEntityType { get; set; }
        public string EntityName { get; set; }
    }
}
