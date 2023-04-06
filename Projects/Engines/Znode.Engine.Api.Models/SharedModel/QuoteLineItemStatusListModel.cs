using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class QuoteLineItemStatusListModel : BaseListModel
    {
        public List<QuoteLineItemStatusModel> QuoteLineItemStatusList { get; set; }

        public QuoteLineItemStatusListModel()
        {
            QuoteLineItemStatusList = new List<QuoteLineItemStatusModel>();
        }
    }
}
