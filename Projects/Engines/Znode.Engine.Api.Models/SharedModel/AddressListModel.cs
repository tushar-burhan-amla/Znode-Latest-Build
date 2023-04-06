using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AddressListModel : BaseListModel
    {
        public List<AddressModel> AddressList { get; set; }
        public string CustomerName { get; set; }
        public bool DontAddUpdateAddress { get; set; }

    }
}
