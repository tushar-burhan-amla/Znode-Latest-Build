
namespace Znode.Engine.Api.Models
{
    public class QuoteLineItemModel : BaseModel
    {
        public int OmsQuoteLineItemId { get; set; }
        public int OmsQuoteId { get; set; }
        public int? ParentOmsQuoteLineItemId { get; set; }
        public decimal? Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal ShippingCost { get; set; }
        public string SKU { get; set; }
        public int? OrderLineItemRelationshipTypeId { get; set; }
        public bool IsAllowedTerritories { get; set; } = true;
        public bool IsPriceEdit { get; set; }
    }
}
