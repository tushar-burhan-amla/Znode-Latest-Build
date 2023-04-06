using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class BannerListModel : BaseListModel
    {
        public List<BannerModel> Banners { get; set; }
    }
}
