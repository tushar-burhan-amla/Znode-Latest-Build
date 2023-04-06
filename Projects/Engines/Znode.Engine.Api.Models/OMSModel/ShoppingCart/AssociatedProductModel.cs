using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AssociatedProductModel : BaseModel
    {
        public int Sequence { get; set; }
        public int ProductId { set; get; }
        public int OmsQuoteId { get; set; }
        public int OmsTemplateId { get; set; }
        public int OmsQuoteLineItemId { get; set; }
        public int OmsTemplateLineItemId { get; set; }
        public int? ParentOmsQuoteLineItemId { get; set; }
        public int? ParentOmsTemplateLineItemId { get; set; }
        public int OrderLineItemRelationshipTypeId { get; set; }

        public decimal Quantity { get; set; }
        public decimal AddOnQuantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? MinimumQuantity { get; set; }
        public decimal? MaximumQuantity { get; set; }

        public string Sku { set; get; }
        public string ExternalId { get; set; }
        public string CustomText { get; set; }
        public string ProductName { set; get; }
        public string CurrencyCode { get; set; }
        public string InStockMessage { get; set; }
        public string CartAddOnDetails { get; set; }
        public string BackOrderMessage { get; set; }
        public string InventoryMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public int? OmsSavedCartLineItemId { get; set; }
        public string CultureCode { get; set; }
        public List<OrderDiscountModel> OrdersDiscount { get; set; }
        public decimal? MaxQuantity { get; set; }
        public decimal? MinQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsObsolete { get; set; }
        public string DefaultInventoryCount { get; set; }

    }
}
