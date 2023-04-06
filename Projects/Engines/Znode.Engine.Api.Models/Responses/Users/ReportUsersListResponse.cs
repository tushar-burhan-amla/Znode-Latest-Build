using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ReportUsersListResponse : BaseListResponse
    {
        public List<ReportUsersModel> UsersList { get; set; }
    }
}
