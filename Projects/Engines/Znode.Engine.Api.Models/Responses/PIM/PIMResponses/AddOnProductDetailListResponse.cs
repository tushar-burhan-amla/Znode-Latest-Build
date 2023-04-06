using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AddOnProductDetailListResponse:BaseListResponse
    {
        public List<AddOnProductDetailModel> AddOnProductDetails { get; set; }
    }
}
