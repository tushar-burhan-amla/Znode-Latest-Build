using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CMSAreaListResponse : BaseListResponse
    {
        public List<CMSAreaModel> CMSAreas { get; set; }
    }
}
