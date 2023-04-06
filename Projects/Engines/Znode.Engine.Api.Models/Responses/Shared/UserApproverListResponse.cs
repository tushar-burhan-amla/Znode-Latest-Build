using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class UserApproverListResponse : BaseListResponse
    {
        public UserApproverListModel UserApproverList { get; set; }
        public List<UserApproverModel> UserApproverModelList { get; set; }
    }
}
