using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PriceListModel : BaseListModel
    {
        public List<PriceModel> PriceList { get; set; }
        public List<PriceProfileModel> Profile { get; set; }
        public bool HasParentAccounts { get; set; }
        public string CustomerName { get; set; }

        public PriceListModel()
        {
            PriceList = new List<PriceModel>();
        }
    }
}
