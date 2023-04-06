using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class ERPConnectorListViewModel : BaseViewModel
    {
        public ERPConnectorListViewModel()
        {
            ERPConnectorControlList = new List<ERPConnectorViewModel>();
        }
        public List<ERPConnectorViewModel> ERPConnectorControlList { get; set; }
        public List<Property> Properties { get; set; }
        public string ERPClassName { get; set; }
       
    }
}
