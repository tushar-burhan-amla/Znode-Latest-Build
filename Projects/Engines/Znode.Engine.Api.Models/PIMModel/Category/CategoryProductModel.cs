using System;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class CategoryProductModel : BaseModel
    {
        public int PimCategoryProductId { get; set; }
        [Required]
        public int? PimCategoryId { get; set; }
        public int? PimCatalogId { get; set; }

        [Required]
        public int PimProductId { get; set; }

        public int? DisplayOrder { get; set; }
        [Required]
        public bool Status { get; set; }
        public string ProductName { get; set; }
        public string ImagePath { get; set; }
        public string ProductType { get; set; }
        public string AttributeFamily { get; set; }
        public string SKU { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Assortment { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public int? Categoryid { get; set; }
    }
}
