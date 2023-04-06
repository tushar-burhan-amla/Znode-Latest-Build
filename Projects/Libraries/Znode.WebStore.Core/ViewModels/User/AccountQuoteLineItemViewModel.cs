using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class AccountQuoteLineItemViewModel : BaseViewModel
    {
        public int OmsQuoteLineItemId { get; set; }
        public int Sequence { get; set; }
        public int OmsQuoteId { get; set; }
        public int? ParentOmsQuoteLineItemId { get; set; }
        public int? OrderLineItemRelationshipTypeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuoteOrderTotal { get; set; }
        public double Price { get; set; }
        public string SKU { get; set; }
        public string CustomText { get; set; }
        public string CartAddOnDetails { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string GroupId { get; set; }
        public List<OrderAttributeModel> Attributes { get; set; }
        public Dictionary<string, object> PersonaliseValueList { get; set; }
        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }
        public int OmsQuoteShipmentId { get; set; }       
        public string Description { get; set; }
        public string UOM { get; set; }
        public decimal? ShippingCost { get; set; }
    }
}