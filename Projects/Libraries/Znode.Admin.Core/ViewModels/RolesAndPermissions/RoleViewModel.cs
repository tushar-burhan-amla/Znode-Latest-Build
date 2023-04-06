using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    //Role View Model
    public class RoleViewModel : BaseViewModel
    {
        public string Id { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorMessageMaxLengthRoleName)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredName)]
        public string Name { get; set; }
        public string TypeOfRole { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsActive, ResourceType = typeof(Admin_Resources))]
        public bool IsActive { get; set; }

        public bool IsSystemDefined { get; set; }
        public List<MenuViewModel> Menus { get; set; }
        public List<PermissionsViewModel> Permissions { get; set; }

        public string PermissionAccessString { get; set; }
        public List<ManageDataModel> MenuPermissions { get; set; }

        public List<RoleMenuAccessMapperViewModel> RoleMenuAccessMapper { get; set; }

        public bool IsAssociated { get; set; }
    }
}