using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class ActionPermissionMapperViewModel : BaseViewModel
    {
        public int AccessPermissionId { get; set; }
        public List<SelectListItem> AccessPermissionList  { get; set; }
        public int ActionId { get; set; }
        public int MenuId { get; set; }        
    }
}