using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class BrandListModel : BaseListModel
    {
        public List<BrandModel> Brands { get; set; }
        public List<PIMAttributeDefaultValueModel> BrandCodes { get; set; }
    }
}
