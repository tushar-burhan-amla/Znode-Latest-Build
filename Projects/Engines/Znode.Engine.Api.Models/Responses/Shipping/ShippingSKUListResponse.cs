using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ShippingSKUListResponse : BaseListResponse
    {
        public List<ShippingSKUModel> ShippingSKUList { get; set; }
    }
}
