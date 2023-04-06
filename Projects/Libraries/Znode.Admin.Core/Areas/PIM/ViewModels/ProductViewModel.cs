using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public int ProductFamily { get; set; }
        public int ProductType { get; set; }
        public int ProductId { get; set; }
        public int LocaleId { get; set; }
        public string AssociatedProducts { get; set; }
        public string ConfigureAttributeIds { get; set; }
        public string ConfigureFamilyIds { get; set; }
        public List<ProductAttributeViewModel> ProductAttributeList { get; set; }
        public List<SelectListItem> AttributeFamilies { get; set; }

        public int? PimCatalogId { get; set; }
        public int? PimCategoryId { get; set; }
        public int? PimCategoryHierarchyId { get; set; }
        public int CopyProductId { get; set; }

        public ProductViewModel()
        {
            ProductAttributeList = new List<ProductAttributeViewModel>();
        }
    }
   
}