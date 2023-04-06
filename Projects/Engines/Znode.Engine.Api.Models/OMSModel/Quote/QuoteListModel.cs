using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class QuoteListModel : BaseListModel
    {
        public List<QuoteModel> Quotes { get; set; }
        public string PortalName { get; set; }
    }
}
