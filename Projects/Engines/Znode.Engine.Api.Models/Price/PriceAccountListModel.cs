using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PriceAccountListModel : BaseListModel
    {
        public List<PriceAccountModel> PriceAccountList { get; set; }
    }
}
