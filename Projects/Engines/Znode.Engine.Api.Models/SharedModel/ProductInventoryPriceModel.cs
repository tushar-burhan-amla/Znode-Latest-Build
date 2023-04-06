using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class ParameterInventoryPriceModel : BaseModel
    {
        /// <summary>
        /// This helps to pass in query parameter (Comma seperated string)
        /// </summary>
        [Required]
        public string Parameter { get; set; }
        public string ProductType { get; set; }
        public int PortalId { get; set; }
        public int LocaleId { get; set; }
        public int CatalogId { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; }
        public bool PriceView { get; set; }
        public string ObsoleteClass { get; set; }
        public string MinQuantity { get; set; }
        public bool IsBundleProduct { get; set; }
        public string BundleProductParentSKU { get; set; }
    }
}
