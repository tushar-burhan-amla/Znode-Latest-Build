using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ReferralCommissionListModel : BaseListModel
    {
        public List<ReferralCommissionModel> ReferralCommissions { get; set; }

        public ReferralCommissionListModel()
        {
            ReferralCommissions = new List<ReferralCommissionModel>();
        }
    }
}
