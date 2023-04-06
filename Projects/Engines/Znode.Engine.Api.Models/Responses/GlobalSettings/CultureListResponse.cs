using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CultureListResponse : BaseListResponse
    {
        public List<CultureModel> Culture { get; set; }
    }
}
