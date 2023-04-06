using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class FacetViewModel : BaseViewModel
    {
        public string AttributeName { set; get; }
        public int ControlTypeId { get; set; }
        public string ControlType { get; set; }
        public int RecordCount { get; set; }
        public List<FacetValueViewModel> AttributeValues { set; get; }
    }
}