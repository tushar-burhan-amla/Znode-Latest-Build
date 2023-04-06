using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProductTypeAssociationListViewModel : BaseViewModel
    {
        public List<ProductTypeAssociationViewModel> AssociatedProducts { get; set; }
    }
}