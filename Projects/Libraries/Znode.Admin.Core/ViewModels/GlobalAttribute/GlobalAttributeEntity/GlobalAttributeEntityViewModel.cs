using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class GlobalAttributeEntityViewModel : BaseViewModel
    {
        public GlobalAttributeEntityViewModel()
        {
            AttributeEntityList = new List<SelectListItem>();
        }
        public int GlobalEntityId { get; set; }
        public string EntityName { get; set; }
        public bool IsActive { get; set; }
        public List<SelectListItem> AttributeEntityList { get; set; }
    }
}
