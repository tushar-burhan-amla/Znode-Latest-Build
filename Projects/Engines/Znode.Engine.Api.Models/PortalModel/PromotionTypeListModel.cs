using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PromotionTypeListModel : BaseListModel
    {
        public PromotionTypeListModel()
        {
            PromotionTypes = new List<PromotionTypeModel>();
        }

        public List<PromotionTypeModel> PromotionTypes { get; set; }        
    }
}
