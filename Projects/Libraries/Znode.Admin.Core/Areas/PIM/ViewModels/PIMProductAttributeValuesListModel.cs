using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PIMProductAttributeValuesListViewModel : BaseViewModel
    {
        public List<PIMProductAttributeValuesViewModel> ProductAttributeValues { get; set; }
        public int PimProductId { get; set; }
        public GridModel GridModel { get; set; }
        public int PriceListId { get; set; }
        public int LocaleId { get; set; }
        public PIMProductAttributeValuesListViewModel()
        {
            ProductAttributeValues = new List<PIMProductAttributeValuesViewModel>();
        }
    }
}