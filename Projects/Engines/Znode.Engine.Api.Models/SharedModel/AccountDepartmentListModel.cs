using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AccountDepartmentListModel : BaseListModel
    {
        public List<AccountDepartmentModel> Departments { get; set; }
    }
}
