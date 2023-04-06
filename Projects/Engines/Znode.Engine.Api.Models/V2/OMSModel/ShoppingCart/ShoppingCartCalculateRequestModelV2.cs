using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models.V2
{
    public class ShoppingCartCalculateRequestModelV2 : BaseModel
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
        public int ShippingOptionId { get; set; }
        public int BillingAddressId { get; set; }
        public int ShippingAddressId { get; set; }
        public string CookieMappingId { get; set; }
        public string GiftCard { get; set; }

        public List<string> PromotionCouponCode { get; set; } = null;        
        public UserAddressV2 ShippingAddress { get; set; } = null;
        public UserAddressV2 BillingAddress { get; set; } = null;

        public bool IsGuest { get; set; } 
    }
}
