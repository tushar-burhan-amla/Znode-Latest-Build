using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ShippingSKUListViewModel :BaseViewModel
    {
        public List<ShippingSKUViewModel> ShippingSKUList { get; set; }
        public GridModel GridModel { get; set; }
        public int ShippingId { get; set; }
        public int ShippingTypeId { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public int? ShippingRuleId { get; set; }
        public string ShippingRuleTypeCode { get; set; }
        public List<ProductDetailsViewModel> ProductDetailList { get; set; }
    }
}