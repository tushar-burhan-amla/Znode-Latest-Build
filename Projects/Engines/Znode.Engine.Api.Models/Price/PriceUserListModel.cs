using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PriceUserListModel : BaseListModel
    {
        public List<PriceUserModel> PriceUserList { get; set; }
    }
}
