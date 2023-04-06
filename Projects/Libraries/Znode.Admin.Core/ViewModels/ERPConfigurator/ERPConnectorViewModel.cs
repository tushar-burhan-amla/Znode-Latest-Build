using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class ERPConnectorViewModel : BaseViewModel
    {
        public ERPConnectorViewModel()
        {
            ERPConnectorControlList = new List<ERPConnectorViewModel>();
        }
        public List<ERPConnectorViewModel> ERPConnectorControlList { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}