using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PriceProfileListModel : BaseListModel
    {
        public List<PriceProfileModel> PriceProfileList { get; set; }
    }
}
