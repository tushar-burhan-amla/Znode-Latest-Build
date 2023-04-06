using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    //Menu Model
    public class MenuModel : BaseModel
    {
        public int? MenuId { get; set; }
        public int? ParentMenuId { get; set; }

        [Range(1, 15, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.MessageSequenceNumberLength)]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.MessageNumericValueAllowed)]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredMessageMenuSequenceNumber)]
        [Display(Name = ZnodeApi_Resources.LabelSequenceNumber, ResourceType = typeof(Api_Resources))]
        public int? MenuSequence { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorMessageMaxLengthMenuName)]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredMessageMenuName)]
        [Display(Name = ZnodeApi_Resources.LabelMenuName, ResourceType = typeof(Api_Resources))]
        public string MenuName { get; set; }
        public string AreaName { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorMessageMaxLengthControllerName)]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredMessageControllerName)]
        [Display(Name = ZnodeApi_Resources.LabelControllerName, ResourceType = typeof(Api_Resources))]
        public string ControllerName { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorMessageMaxLengthActionName)]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredMessageActionName)]
        [Display(Name = ZnodeApi_Resources.LabelActionName, ResourceType = typeof(Api_Resources))]
        public string ActionName { get; set; }

        [Display(Name = ZnodeApi_Resources.LabelIsActive, ResourceType = typeof(Api_Resources))]
        public bool? IsActive { get; set; }

        [Display(Name = ZnodeApi_Resources.LabelIsParentMenu, ResourceType = typeof(Api_Resources))]
        public bool IsParentMenu { get; set; }
        public int RoleMenuId { get; set; }

        public string ParentMenuName { get; set; }

        public string CSSClassName { get; set; }

    }
}
