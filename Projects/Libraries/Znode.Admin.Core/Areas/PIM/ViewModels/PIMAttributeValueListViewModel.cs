using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PIMAttributeValueListViewModel : BaseViewModel
    {
        public List<PIMAttributeValueViewModel> AttributeValues { get; set; }
        public List<PIMAttributeViewModel> Attributes { get; set; }
        public List<BaseDropDownList> baseDropDownList { get; set; }
        public int PimProductId { get; set; }
        public string AttributeCode { get; set; }
    }
}