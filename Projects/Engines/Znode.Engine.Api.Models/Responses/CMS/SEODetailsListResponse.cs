using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SEODetailsListResponse : BaseListResponse
    {
        public List<SEODetailsModel> SEODetails { get; set; }
    }
}
