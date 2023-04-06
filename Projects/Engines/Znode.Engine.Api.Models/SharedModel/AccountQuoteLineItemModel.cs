using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AccountQuoteLineItemModel : SavedCartLineItemModel
    {
        public int OmsQuoteLineItemId { get; set; }
        public int OmsQuoteId { get; set; }
        public int? ParentOmsQuoteLineItemId { get; set; }
        public List<OrderAttributeModel> Attributes { get; set; }
        public Dictionary<string, object> PersonaliseValueList { get; set; }
        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }
        public int OmsQuoteShipmentId { get; set; }    
        public string UOM { get; set; }
        public decimal Price { get; set; }
        public decimal? ShippingCost { get; set; }
        public int Sequence { get; set; }
        public decimal? CustomUnitPrice { get; set; }
    }
}
