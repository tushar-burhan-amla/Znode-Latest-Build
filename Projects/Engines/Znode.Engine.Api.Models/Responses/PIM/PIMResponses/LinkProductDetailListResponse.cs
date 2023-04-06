using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class LinkProductDetailListResponse : BaseListResponse
    {
        public List<LinkProductDetailModel> LinkProducts { get; set; }
    }
}