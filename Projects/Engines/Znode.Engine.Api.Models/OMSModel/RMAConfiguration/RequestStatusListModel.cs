using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class RequestStatusListModel : BaseListModel
    {
        public List<RequestStatusModel> RequestStatusList { get; set; }
    }
}
