using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AccountListModel : BaseListModel
    {
        public List<AccountModel> Accounts { get; set; }       
    }
}
