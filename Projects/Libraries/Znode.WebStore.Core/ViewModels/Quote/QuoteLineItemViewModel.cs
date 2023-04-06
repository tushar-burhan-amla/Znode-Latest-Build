using System;
using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class QuoteLineItemViewModel : BaseViewModel
    {
        public int OmsQuoteLineItemId { get; set; }
        public int OmsQuoteId { get; set; }
        public int? ParentOmsQuoteLineItemId { get; set; }
        public Dictionary<string, object> PersonaliseValueList { get; set; }
        public int? OrderLineItemRelationshipTypeId { get; set; }
        public int OmsQuoteShipmentId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string ParentProductSKU { get; set; }
        public string Sku { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
    }
}
