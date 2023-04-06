using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReferralCommissionListResponse : BaseListResponse
    {
        public List<ReferralCommissionTypeModel> ReferralCommissionTypes { get; set; }

        public List<ReferralCommissionModel> ReferralCommissions { get; set; }
    }
}
