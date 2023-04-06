using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AccountListResponse : BaseListResponse
    {
        //List response for user accounts.
        public List<AccountModel> Accounts { get; set; }

        //List response for user account address.
        public List<AddressModel> AddressList { get; set; }
    }
}
