using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AddressListResponse : BaseListResponse
    {
        public List<AddressModel> AddressList { get; set; }
        public string CustomerName { get; set; }
    }
}
