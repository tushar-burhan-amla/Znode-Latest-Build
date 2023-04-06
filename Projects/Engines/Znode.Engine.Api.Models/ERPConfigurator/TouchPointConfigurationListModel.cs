using System.Collections.Generic;
using System.Reflection;

namespace Znode.Engine.Api.Models
{
    public class TouchPointConfigurationListModel : BaseListModel
    {
        public List<TouchPointConfigurationModel> TouchPointConfigurationList { get; set; }
        public List<TouchPointConfigurationModel> SchedulerLogList { get; set; }
        public MethodInfo[] ZnodeArrayMethodInfo { get; set; }
        public TouchPointConfigurationListModel()
        {
            TouchPointConfigurationList = new List<TouchPointConfigurationModel>();
            SchedulerLogList = new List<TouchPointConfigurationModel>();
        }        
    }
}
