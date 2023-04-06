using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ERPConnectorControlListResponse : BaseListResponse
    {
        public List<ERPConnectorControlModel> ERPConnectorControls { get; set; }
    }
}
