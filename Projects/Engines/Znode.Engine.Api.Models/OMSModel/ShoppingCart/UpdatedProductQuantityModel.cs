using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class UpdatedProductQuantityModel : BaseModel
    {
        public string SKU { get; set; }
        public string AddOnProductSKUs { get; set; }
        public string AutoAddonSKUs { get; set; }
        public string BundleProductSKUs { get; set; }
        public string ConfigurableProductSKUs { get; set; }
        public decimal Quantity { get; set; }
        public string PersonaliseValues { get; set; }
        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }
    }
}
