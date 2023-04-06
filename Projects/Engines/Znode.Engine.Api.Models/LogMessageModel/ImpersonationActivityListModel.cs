using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ImpersonationActivityListModel : BaseListModel
    {
        public List<ImpersonationActivityLogModel> LogActivityList { get; set; }

        public ImpersonationActivityListModel()
        {
            LogActivityList = new List<ImpersonationActivityLogModel>();
        }
    }
}
