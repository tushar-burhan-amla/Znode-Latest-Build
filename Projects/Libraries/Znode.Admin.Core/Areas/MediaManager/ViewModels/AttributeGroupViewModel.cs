using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Api.Models;

using Znode.Libraries.Resources;
namespace Znode.Engine.Admin.ViewModels
{
    public class AttributeGroupViewModel : BaseViewModel
    {
        public int MediaAttributeGroupId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredAttributeGroup, ErrorMessageResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericStartWithAlphabet, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AttributeGroupCodeLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelGroupCode, ResourceType = typeof(Admin_Resources))]
        public string GroupCode { get; set; }

        public string AttributeGroupName { get; set; }

        public bool IsSystemDefined { get; set; }

        [Display(Name = ZnodeAttributes_Resources.LabelDisplayOrder, ResourceType = typeof(Attributes_Resources))]
        [Range(1, 999, ErrorMessageResourceName = ZnodeAttributes_Resources.ErrorDisplayOrder, ErrorMessageResourceType = typeof(Attributes_Resources))]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceName = ZnodeAttributes_Resources.InvalidDisplayOrder, ErrorMessageResourceType = typeof(Attributes_Resources))]
        public int? DisplayOrder { get; set; }

        public List<AttributesDataModel> AttributeModel { get; set; }
        public NavigationViewModel NavigationViewModel { get; set; }

        public string ViewModeType { get; set; }
    }
}