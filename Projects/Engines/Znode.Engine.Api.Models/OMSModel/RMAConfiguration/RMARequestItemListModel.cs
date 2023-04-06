using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class RMARequestItemListModel : BaseListModel
    {       
        public RMARequestItemListModel()
        {
            RMARequestItemList = new List<RMARequestItemModel>();
        }
        public List<RMARequestItemModel> RMARequestItemList { get; set; }
    }
}
