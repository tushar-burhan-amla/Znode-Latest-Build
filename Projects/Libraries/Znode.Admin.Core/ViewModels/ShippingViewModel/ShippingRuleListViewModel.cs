using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ShippingRuleListViewModel : BaseViewModel
    {
        public List<ShippingRuleViewModel> ShippingRuleList { get; set; }
        public GridModel GridModel { get; set; }
        public string ClassName { get; set; }
        public int ShippingId { get; set; }
        public int ShippingTypeId { get; set; }
        public string Name { get; set; }
    }
}