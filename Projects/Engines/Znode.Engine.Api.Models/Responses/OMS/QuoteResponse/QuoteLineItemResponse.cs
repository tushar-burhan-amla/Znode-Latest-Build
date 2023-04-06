using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class QuoteLineItemResponse : BaseResponse
    {
        public List<QuoteLineItemModel> QuoteLineItems { get; set; }
    }
}
