using System;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models.V2
{
    public class ShoppingCartItemModelV2 : BaseModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "ShoppingCartItems: {0} cannot be less than {1}")]
        public int OmsSavedcartLineItemId { get; set; }

        [Range(1, 1000)]
        public decimal Quantity { get; set; }

        [Range(typeof(decimal), "0", "1000000", ErrorMessage = "ShoppingCartItems: {0} cannot be less than {1}")]
        public decimal UnitPrice { get; set; }
        
        [Required]
        public string SKU { get; set; }        
        public string ExternalId { get; set; }
        public string ChildProductSKU { get; set; }
        public string AddOnProductSKUs { get; set; }
        public string CartDescription { get; set; }
        public string ProductName { get; set; }
        public string ImagePath { get; set; }
        public string UOM { get; set; }
        public string CustomText { get; set; }

    }
}