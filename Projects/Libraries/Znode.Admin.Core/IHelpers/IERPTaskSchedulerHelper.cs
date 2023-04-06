using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Admin.Helpers
{
    public interface IERPTaskSchedulerHelper
    {
        string GetSchedulerTitle(string ConnectorTouchPoints, string schedulerCallFor="");
    }
}
