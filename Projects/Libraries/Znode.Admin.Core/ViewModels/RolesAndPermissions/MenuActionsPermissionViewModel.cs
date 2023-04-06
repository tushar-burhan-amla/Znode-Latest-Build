using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class MenuActionsPermissionViewModel : BaseViewModel
    {
        public List<SelectListItem> ActionList { get; set; }        
        public List<ActionPermissionMapperViewModel> ActionPermissionList { get; set; }
        public int MenuId { get; set; }
        public int ActionId { get; set; }
        public int AccessPermissionId { get; set; }
        public string MenuName { get; set; }
        public string ActionPermissions { get; set; }
    }
}