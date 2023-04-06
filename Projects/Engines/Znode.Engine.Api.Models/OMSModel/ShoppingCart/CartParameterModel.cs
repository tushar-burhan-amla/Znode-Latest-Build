namespace Znode.Engine.Api.Models
{
    public class CartParameterModel : BaseModel
    {
        public string CookieMappingId { get; set; }
        public int? CookieId { get; set; }
        public int? UserId { get; set; }
        public int LocaleId { get; set; }
        public int PortalId { get; set; }
        public int PublishedCatalogId { get; set; }
        public int? OmsOrderId { get; set; }
        public int? ShippingId { get; set; }
        public string ShippingCountryCode { get; set; }
        public int OmsQuoteId { get; set; }
        public bool IsQuote { get; set; }
        public int ProfileId { get; set; }
        public bool IsOldOrder { get; set; }
    }
}
