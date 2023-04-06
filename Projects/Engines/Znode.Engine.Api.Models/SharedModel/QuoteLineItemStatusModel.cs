namespace Znode.Engine.Api.Models
{
    public class QuoteLineItemStatusModel : BaseModel
    {
        public int? ParentQuoteLineItemId { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}