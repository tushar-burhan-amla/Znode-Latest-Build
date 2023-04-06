using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Api.Models.V2.DataAnnotations;

namespace Znode.Engine.Api.Models.V2
{
    public class ShoppingCartModelV2 : BaseModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "{0} cannot be empty or less than {1}.")]
        public int PortalId { get; set; }        
                
        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be empty or less than {1}.")]
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "{0} cannot be empty or less than {1}.")]
        public int PublishedCatalogId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int OmsSavedCartId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int LocaleId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int ProfileId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int ShippingId { get; set; }

        public decimal GiftCardAmount { get; set; }
        public decimal GiftCardBalance { get; set; }
        public decimal OrderLevelShipping { get; set; }
        public decimal OrderLevelTaxes { get; set; }
        public decimal OrderLevelDiscount { get; set; }
        public decimal Discount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TaxCost { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        
        public string CookieMappingId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySuffix { get; set; }
        public string GiftCardNumber { get; set; }

        public bool IsQuoteOrder { get; set; }
        public bool MultipleShipToEnabled { get; set; }

        [ValidateList]
        public List<ShoppingCartItemModelV2> ShoppingCartItems { get; set; }
    }    
}
