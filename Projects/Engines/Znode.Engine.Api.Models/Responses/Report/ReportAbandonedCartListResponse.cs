using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportAbandonedCartListResponse : BaseListResponse
    {
        public List<ReportAbandonedCartModel> AbandonedCartList { get; set; }
    }
}
