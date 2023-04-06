using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    //Menu View Model
    public class MenuViewModel : BaseViewModel
    {
        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSequenceNumber, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [Range(1, 15, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.MessageSequenceNumberLength)]
        [RegularExpression(AdminConstants.EmailValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.MessageNumericValueAllowed)]
        public int? MenuSequence { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorMessageMaxLengthMenuName)]
        [Display(Name = ZnodeAdmin_Resources.LabelMenuName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        public string MenuName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAreaName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(20, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorMessageMaxLengthAreaName)]
        public string AreaName { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorMessageMaxLengthControllerName)]
        [Display(Name = ZnodeAdmin_Resources.LabelControllerName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        public string ControllerName { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorMessageMaxLengthActionName)]
        [Display(Name = ZnodeAdmin_Resources.LabelActionName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        public string ActionName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsActive, ResourceType = typeof(Admin_Resources))]
        public bool IsActive { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsParentMenu, ResourceType = typeof(Admin_Resources))]
        public bool IsParentMenu { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelParentMenuList, ResourceType = typeof(Admin_Resources))]
        public List<MenuViewModel> ParentMenuList { get; set; }

        public string ParentMenuName { get; set; }
        public int RoleMenuId { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorMessageMaxLengthControllerName)]
        [Display(Name = ZnodeAdmin_Resources.LabelCssClassName, ResourceType = typeof(Admin_Resources))]
        public string CSSClassName { get; set; }

       
    }
}