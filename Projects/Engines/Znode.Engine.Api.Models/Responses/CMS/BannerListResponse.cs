using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class BannerListResponse : BaseListResponse
    {
        public List<BannerModel> Banners { get; set; }
    }
}
