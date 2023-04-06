using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models.V2
{
    public class CreateOrderModelV2 : BaseModel
    {
        public int PortalId { get; set; }
        public int? UserId { get; set; }
        public int LocaleId { get; set; }
        public int? ProfileId { get; set; }
        public int PublishedCatalogId { get; set; }
        public int BillingAddressId { get; set; }
        public int ShippingAddressId { get; set; }
        public DateTime OrderDate { get; set; }
        public int PaymentOptionId { get; set; }
        public string PaymentTransactionId { get; set; }
        public int ShippingOptionId { get; set; }
        public string CookieMappingId { get; set; }
        public List<string> PromotionCouponCode { get; set; } = null;
        public string GiftCard { get; set; }
        public bool IsGuest { get; set; }
        [MinLength(4, ErrorMessage = "Invalid Credit card number.")]
        [MaxLength(4, ErrorMessage = "Invalid Credit card number.")]
        public string CreditCardNumber { get; set; }
        public string CreditCardType { get; set; }
        public string OrderNumber { get; set; }
    }
}
