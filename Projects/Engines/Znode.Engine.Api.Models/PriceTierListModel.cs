using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PriceTierListModel : BaseListModel
    {
        public List<PriceTierModel> TierPriceList { get; set; }
    }
}
