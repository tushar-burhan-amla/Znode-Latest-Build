using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PromotionListModel : BaseListModel
    {
        public List<PromotionModel> PromotionList { get; set; }
    }
}
