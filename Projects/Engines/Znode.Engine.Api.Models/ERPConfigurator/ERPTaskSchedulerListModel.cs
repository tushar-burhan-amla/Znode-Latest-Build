using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ERPTaskSchedulerListModel : BaseListModel
    {
        public List<ERPTaskSchedulerModel> ERPTaskSchedulerList { get; set; }
        public ERPTaskSchedulerListModel()
        {
            ERPTaskSchedulerList = new List<ERPTaskSchedulerModel>();
        }
    }
}
