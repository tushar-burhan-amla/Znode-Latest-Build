using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ApproverLevelListModel : BaseListModel
    {
        public ApproverLevelListModel()
        {
            ApproverLevelList = new List<ApproverLevelModel>();            
        }

        public List<ApproverLevelModel> ApproverLevelList { get; set; }
    }
}