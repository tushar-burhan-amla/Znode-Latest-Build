using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreCaseRequestListModel : BaseListModel
    {
        public List<WebStoreCaseRequestModel> CaseRequestList { get; set; }
    }
}
