using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreAccountResponse : BaseListResponse
    {
        public AccountModel Account { get; set; }

        public AddressModel AccountAddress { get; set; }

        public List<AddressModel> UserAddressList { get; set; }
    }
}
