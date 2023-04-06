using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AccountDepartmentListResponse : BaseListResponse
    {
        public List<AccountDepartmentModel> Departments { get; set; }
    }
}
