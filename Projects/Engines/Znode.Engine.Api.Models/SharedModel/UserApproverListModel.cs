using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class UserApproverListModel : BaseListModel
    {
        public int? AccountId { get; set; }
        public int? PortalId { get; set; }
        public List<UserApproverModel> UserApprovers { get; set; }
        public int? AccountPermissionAccessId { get; set; }
        public int? AccountUserPermissionId { get; set; }
    }
}
