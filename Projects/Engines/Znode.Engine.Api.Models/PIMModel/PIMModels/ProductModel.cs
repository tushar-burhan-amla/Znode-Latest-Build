using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProductModel : BaseModel
    {
        public int? ProductFamily { get; set; }
        public int ProductType { get; set; }
        public int ProductId { get; set; }
        public int LocaleId { get; set; }
        public string AssociatedProducts { get; set; }
        public string ConfigureAttributeIds { get; set; }
        public string ConfigureFamilyIds { get; set; }
        public List<ProductAttributeModel> ProductAttributeList { get; set; }

        public int? PimCatalogId { get; set; }
        public int? PimCategoryId { get; set; }
        public int? PimCategoryHierarchyId { get; set; }
        public int CopyProductId { get; set; }

        public ProductModel()
        {
            ProductAttributeList = new List<ProductAttributeModel>();
        }
    }
}
