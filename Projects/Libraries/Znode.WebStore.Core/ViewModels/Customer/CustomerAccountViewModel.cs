using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.WebStore.ViewModels
{
    public class CustomerAccountViewModel : CustomerViewModel
    {
        public List<SelectListItem> AccountDepartmentList { get; set; }
        
    }
}