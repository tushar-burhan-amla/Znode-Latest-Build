using Znode.Engine.Admin.Agents;
using Znode.Engine.Api.Client;
using System.Web;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Admin.Helpers
{
    public class ERPTaskSchedulerHelper: IERPTaskSchedulerHelper
    {
        public virtual string GetSchedulerTitle(string ConnectorTouchPoints, string schedulerCallFor="")
        {
            IERPTaskSchedulerAgent _erpTaskSchedulerAgent = GetService<IERPTaskSchedulerAgent>();//new ERPTaskSchedulerAgent(new ERPTaskSchedulerClient());
            if (!string.IsNullOrEmpty(ConnectorTouchPoints))
            {
                ConnectorTouchPoints = HttpUtility.UrlDecode(ConnectorTouchPoints);
                int erpTaskSchedulerId = _erpTaskSchedulerAgent.GetSchedulerIdByTouchPointName(ConnectorTouchPoints, schedulerCallFor);
                return erpTaskSchedulerId == 0 ? AdminConstants.CreateScheduler : AdminConstants.UpdateScheduler;
            }
            else
            {
                return AdminConstants.CreateScheduler;
            }
        }
    }
}
